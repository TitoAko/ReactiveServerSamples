using CoreLibrary.Messaging;
using CoreLibrary.Utilities;
using System.Net.Sockets;
using System.Text;

namespace CoreLibrary.Communication.UdpCommunication
{
    /// <summary>
    /// Handles sending serialized messages over UDP.
    /// </summary>
    public class UdpSender
    {
        private readonly UdpClient _udpClient;
        private readonly Configuration _configuration;

        /// <summary>
        /// Initializes a new instance of the sender with the given config.
        /// </summary>
        public UdpSender(Configuration configuration)
        {
            _configuration = configuration;
            _udpClient = new UdpClient();
        }

        /// <summary>
        /// Sends a message asynchronously using UDP.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="cancellationToken">Token for graceful cancellation.</param>
        public async Task SendMessageAsync(Message message, CancellationToken cancellationToken = default)
        {
            byte[] messageBytes = Encoding.ASCII.GetBytes(message.Content);

            if (cancellationToken.IsCancellationRequested)
                return;

            await _udpClient.SendAsync(messageBytes, messageBytes.Length, _configuration.IpAddress, _configuration.Port)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Disposes the underlying UDP client.
        /// </summary>
        public void Dispose()
        {
            _udpClient.Dispose();
        }
    }
}
