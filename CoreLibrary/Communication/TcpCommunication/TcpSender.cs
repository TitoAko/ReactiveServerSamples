using System.Net.Sockets;
using System.Text;
using System.Text.Json;

using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace CoreLibrary.Communication.TcpCommunication
{
    public sealed class TcpSender : IAsyncDisposable
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

        // 🔹 NEW — expose the NetworkStream once the connection is established
        public NetworkStream Stream
        {
            get
            {
                if (!_connected) // lazily establish the first time it’s requested
                {
                    _tcpClient.Connect(_configuration.TargetAddress, _configuration.Port);
                    _connected = true;
                }

                return _tcpClient.GetStream();
            }
        }

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
            // ⬇ ensure we have a live stream (uses the property above)
            var stream = Stream;

            string json = JsonSerializer.Serialize(message, _jsonSerializerOptions);
            var buffer = Encoding.UTF8.GetBytes(json);

            await stream.WriteAsync(buffer, token).ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;

            _tcpClient.Dispose();  // synchronous close is fine
            await Task.CompletedTask;
        }
    }
}