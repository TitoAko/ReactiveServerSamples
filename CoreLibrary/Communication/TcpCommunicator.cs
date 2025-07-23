using CoreLibrary.Messaging;
using CoreLibrary.Utilities;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace CoreLibrary.Communication
{
    /// <summary>
    /// Minimal TCP transport. Listens on the configured port and relays JSON
    /// chat packets.  Useful for smoke tests; production TODOs marked.
    /// </summary>
    public sealed class TcpCommunicator : ICommunicator
    {
        private readonly TcpListener _listener;
        private readonly Configuration _cfg;
        private readonly CancellationTokenSource _cts = new();

        private TcpClient? _client;

        public event EventHandler<Message>? MessageReceived;

        public TcpCommunicator(Configuration cfg)
        {
            _cfg = cfg;
            _listener = new TcpListener(
                IPAddress.Parse(cfg.IpAddress), cfg.Port);
        }

        /* -------------------------------------------------------------- */
        /* ICommunicator                                                  */
        /* -------------------------------------------------------------- */

        public Task SendMessageAsync(Message message, CancellationToken token = default)
            => _client is null ? Task.CompletedTask : SendAsyncInternal(message, token);

        public Task StartAsync(CancellationToken token = default)
        {
            var linked = CancellationTokenSource.CreateLinkedTokenSource(token, _cts.Token);

            _listener.Start();
            Console.WriteLine($"[TCP] Listening on {_cfg.BindAddress}:{_cfg.Port}");

            _ = AcceptLoopAsync(linked.Token);   // fire-and-forget
            return Task.CompletedTask;           // return immediately
        }

        public void Dispose()
        {
            _cts.Cancel();
            try { _client?.Close(); } catch { }
            _listener.Stop();
        }

        /* -------------------------------------------------------------- */
        /* Internals                                                      */
        /* -------------------------------------------------------------- */

        private async Task AcceptLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    _client = await _listener.AcceptTcpClientAsync(token)
                                                .ConfigureAwait(false);
                    Console.WriteLine("[TCP] Client accepted.");
                    _ = ReceiveLoopAsync(_client, token);   // detach
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    Console.WriteLine($"[TCP] Accept error: {ex.Message}");
                    await Task.Delay(500, token);
                }
            }
        }

        private async Task ReceiveLoopAsync(TcpClient client, CancellationToken token)
        {
            using var reader = new StreamReader(client.GetStream(), Encoding.UTF8);
            while (!token.IsCancellationRequested)
            {
                string? line;
                try { line = await reader.ReadLineAsync().WaitAsync(token); }
                catch { break; }

                if (line is null) break;                       // remote closed

                try
                {
                    var opts = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Converters = { new MessageTypeConverter() }
                    };
                    var msg = JsonSerializer.Deserialize<Message>(line, opts);
                    if (msg != null) MessageReceived?.Invoke(this, msg);
                }
                catch
                {
                    // fallback plain-text -> chat
                    var fallback = new Message(_cfg.ClientId, line, MessageType.Chat);
                    MessageReceived?.Invoke(this, fallback);
                }
            }
        }

        private async Task SendAsyncInternal(Message message, CancellationToken token)
        {
            try
            {
                var opts = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new MessageTypeConverter() }
                };
                string json = JsonSerializer.Serialize(message, opts) + "\n";
                byte[] data = Encoding.UTF8.GetBytes(json);
                await _client!.GetStream().WriteAsync(data.AsMemory(0, data.Length), token)
                                         .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TCP] Send error: {ex.Message}");
            }
        }
    }
}
