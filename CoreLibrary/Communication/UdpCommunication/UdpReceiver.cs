using System.Net;
using System.Net.Sockets;
using System.Text.Json;

using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace CoreLibrary.Communication.UdpCommunication
{
    public sealed class UdpReceiver : IAsyncDisposable
    {
        private readonly UdpClient _client;
        private bool _disposed;
        private readonly Configuration _cfg;

        public event EventHandler<Message>? Received;

        public UdpReceiver(Configuration cfg)
        {
            _cfg = cfg ?? throw new ArgumentNullException(nameof(cfg));
            _client = new UdpClient(new IPEndPoint(
                IPAddress.Parse(_cfg.BindAddress), _cfg.Port));
        }

        public async Task ListenAsync(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                var result = await _client.ReceiveAsync(token);
                if (result.Buffer.Length <= _cfg.UdpMaxPayload)
                {
                    var msg = JsonSerializer.Deserialize<Message>(result.Buffer)!;
                    Received?.Invoke(this, msg);
                }
            }
        }

        public ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                _disposed = true;
                _client.Dispose();
            }
            return ValueTask.CompletedTask;
        }
    }
}