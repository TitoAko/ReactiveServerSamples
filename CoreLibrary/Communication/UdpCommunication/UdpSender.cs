using CoreLibrary.Messaging;
using CoreLibrary.Utilities;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace CoreLibrary.Communication.UdpCommunication
{
    /// <summary>
    /// Handles sending serialized messages over UDP.
    /// </summary>
    public class UdpSender : IDisposable
    {
        private readonly UdpClient _udpClient;
        private readonly IPEndPoint _remoteEndPoint;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        /// <summary>
        /// Initializes a new instance of the sender with the given config.
        /// </summary>
        public UdpSender(Configuration cfg, int? remotePort = null)
        {
            var host = cfg.TargetAddress;
            if (host == "0.0.0.0" || host == "::0")
                throw new ArgumentException("Cannot send to an unspecified address", nameof(cfg.TargetAddress));

            _udpClient = new UdpClient();

            _remoteEndPoint = new IPEndPoint(
                Dns.GetHostAddresses(host).First(),     // resolves ‘server’ inside Docker
                remotePort ?? cfg.Port);

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

            string json = JsonSerializer.Serialize(message, _jsonSerializerOptions);

            byte[] buffer = Encoding.UTF8.GetBytes(json);

            if (buffer.Length > 60_000)
                throw new ArgumentException("UDP payload exceeds 60 kB.");

            await _udpClient.SendAsync(buffer.AsMemory(),
                                                    _remoteEndPoint,
                                                    cancellationToken)
                                        .ConfigureAwait(false); if (cancellationToken.IsCancellationRequested)
                return;
        }

        /// <summary>
        /// Disposes the underlying UDP client.
        /// </summary>
        public void Dispose() => _udpClient.Dispose();
    }
}
