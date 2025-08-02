using System.Net.Sockets;
using System.Text;
using System.Text.Json;

using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace CoreLibrary.Communication.UdpCommunication
{
    public sealed class UdpSender : IAsyncDisposable
    {
        private readonly UdpClient _client;
        private bool _disposed;

        public UdpSender(Configuration cfg, int? remotePort = null)
        {
            _client = new UdpClient(cfg.BindAddress, 0);
            _client.Connect(cfg.TargetAddress, remotePort ?? cfg.Port);
        }

        public async Task SendAsync(Message msg, CancellationToken token = default)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msg));
            if (bytes.Length > 60_000)
            {
                throw new ArgumentException("UDP payload limit 60 kB exceeded.", nameof(msg));
            }

            await _client.SendAsync(bytes, token);
        }

        public ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                _disposed = true;
                _client.Dispose();                    // synchronous close
            }
            return ValueTask.CompletedTask;       // satisfy the async contract
        }
    }
}
