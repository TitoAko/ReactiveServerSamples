using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace CoreLibrary.Communication.TcpCommunication
{
    /// <summary>
    /// Thin façade that owns a <see cref="TcpListener"/> (server side) *and* a
    /// <see cref="TcpSender"/> (client side).  Tests await <see cref="Started"/>
    /// to know when the listener socket is bound.
    /// </summary>
    public sealed class TcpCommunicator : ICommunicator, IDisposable
    {
        private readonly TcpListener _listener;
        private readonly TcpSender _sender;
        private readonly TaskCompletionSource _listenerStartedTcs =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task Started => _listenerStartedTcs.Task;     // awaited by tests

        private readonly JsonSerializerOptions _json = new()
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

        #region ICommunicator
        public async Task StartAsync(CancellationToken token = default)
        {
            _ = Task.Run(() => AcceptLoopAsync(token), token);
            await Started;                                   // wait until bound
        }

        public Task SendMessageAsync(Message m, CancellationToken t = default) =>
            _sender.SendAsync(m, t);
        #endregion
        #region Listener loop
        private async Task AcceptLoopAsync(CancellationToken token)
        {
            _listener.Start();
            _listenerStartedTcs.TrySetResult();              // signal “ready”

            try
            {
                while (!token.IsCancellationRequested)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync(token);
                    _ = HandleClientAsync(client, token);    // fire-and-forget
                }
            }
            catch (OperationCanceledException) { /* normal shutdown */ }
            catch (ObjectDisposedException) { /* listener stopped */ }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            using var stream = client.GetStream();
            var buffer = new byte[4096];

            try
            {
                while (!token.IsCancellationRequested)
                {
                    int read = await stream.ReadAsync(buffer, token);
                    if (read == 0) break;                   // remote closed

                    string json = Encoding.UTF8.GetString(buffer, 0, read);
                    var msg = JsonSerializer.Deserialize<Message>(json, _json)!;
                    MessageReceived?.Invoke(this, msg);
                }
            }
            catch (OperationCanceledException) { /* ignore */ }
            finally
            {
                client.Dispose();
            }
        }
        #endregion
        #region Cleanup
        public void Dispose()
        {
            _listener.Stop();           // causes AcceptTcpClientAsync to throw
            _sender.Dispose();
        }
        #endregion
    }
}