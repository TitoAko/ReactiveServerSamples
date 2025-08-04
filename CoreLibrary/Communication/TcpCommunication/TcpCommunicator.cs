using System.Net;
using System.Net.Sockets;
using System.Text.Json;

using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace CoreLibrary.Communication.TcpCommunication
{
    /// <summary>
    /// Owns a <see cref="TcpListener"/> (server-side) and a <see cref="TcpSender"/> (client-side).
    /// Frames are 4-byte length-prefixed.
    /// </summary>
    public sealed class TcpCommunicator : ICommunicator, IAsyncDisposable
    {
        private readonly TcpListener _listener;
        private readonly TcpSender _sender;
        private readonly CancellationTokenSource _cts = new();
        private Task? _acceptLoop;

        // TaskCompletionSource to signal when the listener has started
        private readonly TaskCompletionSource _listenerStartedTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public Task Started => _listenerStartedTcs.Task;     // awaited by tests

        private readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new MessageTypeConverter() }
        };

        public event EventHandler<Message>? MessageReceived;

        public TcpCommunicator(Configuration cfg)
        {
            _listener = new TcpListener(IPAddress.Parse(cfg.BindAddress), cfg.Port);
            _sender = new TcpSender(cfg);
        }

        // --------------------------------------------------------------------
        //  ICommunicator
        // --------------------------------------------------------------------
        public Task StartAsync(CancellationToken token = default)
        {
            _acceptLoop = AcceptLoopAsync(_cts.Token);
            return Task.CompletedTask;                  // fire-and-forget
        }

        public Task SendMessageAsync(Message msg, CancellationToken token = default)
        {
            var payload = JsonSerializer.SerializeToUtf8Bytes(msg, _jsonOpts);
            return WriteFrameAsync(_sender.Stream, payload, token);
        }

        // --------------------------------------------------------------------
        //  Listener loop
        // --------------------------------------------------------------------
        private async Task AcceptLoopAsync(CancellationToken token)
        {
            _listener.Start();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var client = await _listener.AcceptTcpClientAsync(token);
                    _ = HandleClientAsync(client, token);      // detached
                }
            }
            catch (OperationCanceledException) { /* normal shutdown */ }
            catch (ObjectDisposedException) { /* listener closed   */ }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            await using var stream = client.GetStream();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    // 🔹 read exactly one length-prefixed frame
                    var frame = await ReadFrameAsync(stream, token);

                    // 🔹 turn the raw bytes back into a Message
                    var msg = JsonSerializer.Deserialize<Message>(frame, _jsonOpts)!;

                    MessageReceived?.Invoke(this, msg);
                }
            }

            catch (Exception ex) when (ex is EndOfStreamException ||
                                                ex is OperationCanceledException ||
                                                ex is IOException)
            {
                //  ignore these exceptions, they are normal connection terminations
            }
            finally
            {
                client.Dispose();
            }
        }

        // --------------------------------------------------------------------
        //  Framing helpers (4-byte little-endian length)
        // --------------------------------------------------------------------
        private static async Task WriteFrameAsync(Stream s, ReadOnlyMemory<byte> buf, CancellationToken t)
        {
            byte[] len = BitConverter.GetBytes(buf.Length);   // 4-byte little-endian

            await s.WriteAsync(len, t).ConfigureAwait(false);
            await s.WriteAsync(buf, t).ConfigureAwait(false);
        }

        private static async Task ReadExactAsync(Stream s, byte[] buf, CancellationToken t)
        {
            int offset = 0;
            while (offset < buf.Length)
            {
                int n = await s.ReadAsync(buf.AsMemory(offset), t).ConfigureAwait(false);
                if (n == 0)
                {
                    throw new EndOfStreamException();   // remote closed
                }

                offset += n;
            }
        }

        private static async Task<byte[]> ReadFrameAsync(Stream s, CancellationToken t = default)
        {
            var lenBuf = new byte[4];
            await ReadExactAsync(s, lenBuf, t);

            int len = BitConverter.ToInt32(lenBuf);
            if (len < 0 || len > 1_048_576)
            {
                throw new InvalidDataException($"Frame length {len} is invalid.");
            }

            var payload = new byte[len];
            await ReadExactAsync(s, payload, t);
            return payload;
        }
        // --------------------------------------------------------------------
        //  Cleanup
        // --------------------------------------------------------------------
        public async ValueTask DisposeAsync()
        {
            _cts.Cancel();
            if (_acceptLoop is not null)
            {
                await _acceptLoop;             // wait for graceful shutdown
            }
            _listener.Stop();
            await _sender.DisposeAsync();
        }
    }
}
