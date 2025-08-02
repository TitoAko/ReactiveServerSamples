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

        public event EventHandler<Message>? Received;

        public UdpReceiver(Configuration cfg)
        {
            _client = new UdpClient(new IPEndPoint(
                IPAddress.Parse(cfg.BindAddress), cfg.Port));
        }

        public async Task ListenAsync(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                var result = await _client.ReceiveAsync(token);
                var msg = JsonSerializer.Deserialize<Message>(result.Buffer)!;
                Received?.Invoke(this, msg);
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