using System.Net.Sockets;
using System.Text;
using System.Text.Json;

using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace CoreLibrary.Communication.UdpCommunication
{
    /// <summary>
    /// Handles sending serialized messages over UDP.
    /// </summary>
    public class UdpSender : IDisposable
    {
        private readonly UdpClient _udpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly Configuration _configurationg;
        private readonly int _remotePort;
        private bool _disposed;
        private bool _connected;

        /// <summary>
        /// Initializes a new instance of the sender with the given config.
        /// </summary>
        public UdpSender(Configuration configuration, int? remotePort = null)
        {
            _configurationg = configuration;
            _remotePort = remotePort ?? configuration.Port;     // ← use other side’s port when supplied

            var host = configuration.TargetAddress;
            if (host == "0.0.0.0" || host == "::0")
            {
                throw new ArgumentException("Cannot send to an unspecified address", nameof(_configurationg.TargetAddress));
            }

            _udpClient = new UdpClient();

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // <- matches receiver
                Converters = { new MessageTypeConverter() } // <- enum ↔ string
            };
        }

        /// <summary>
        /// Sends a message asynchronously using UDP.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="cancellationToken">Token for graceful cancellation.</param>
        public async Task SendMessageAsync(Message message, CancellationToken cancellationToken = default)
        {
            if (message.Content.Length == 0)
            {
                throw new ArgumentException("Empty payload", nameof(message));
            }

            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(UdpSender));
            }

            if (!_connected)
            {
                _udpClient.Connect(_configurationg.TargetAddress, _remotePort);
                _connected = true;
            }

            string serializedMessage = JsonSerializer.Serialize(message, _jsonSerializerOptions);

            byte[] messageBuffer = Encoding.UTF8.GetBytes(serializedMessage);

            if (messageBuffer.Length > 60_000)
            {
                throw new ArgumentException("UDP payload exceeds 60 kB.");
            }

            await _udpClient.SendAsync(messageBuffer.AsMemory(), cancellationToken)
                             .ConfigureAwait(false);   // use connected socket
            return;
        }

        /// <summary>
        /// Disposes the underlying UDP client.
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
            _udpClient.Dispose();
        }
    }
}
