using System.Net.Sockets;
using System.Text;
using System.Text.Json;

using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace CoreLibrary.Communication.TcpCommunication
{
    public sealed class TcpSender : IDisposable
    {
        private bool _disposed;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new MessageTypeConverter() }
        };
        private readonly TcpClient _tcpClient = new();
        private readonly Configuration _configuration;
        private bool _connected;

        public TcpSender(Configuration cfg)
        {
            _configuration = cfg;
        }

        public async Task SendAsync(Message message, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(message.Content))
            {
                throw new ArgumentException("Empty payload", nameof(message));
            }

            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(TcpSender));
            }

            if (!_connected)
            {
                _tcpClient.Connect(_configuration.TargetAddress, _configuration.Port);
                _connected = true;
            }

            string serializedMessage = JsonSerializer.Serialize(message, _jsonSerializerOptions);
            byte[] buffer = Encoding.UTF8.GetBytes(serializedMessage);
            await _tcpClient.GetStream().WriteAsync(buffer, token).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _disposed = true;
            _tcpClient.Dispose();
        }
    }
}