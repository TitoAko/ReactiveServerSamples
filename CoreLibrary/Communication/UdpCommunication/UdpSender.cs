using CoreLibrary.Messaging;
using CoreLibrary.Utilities;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CoreLibrary.Communication.UdpCommunication
{
    public class UdpSender
    {
        private readonly UdpClient _udpClient;
        private readonly Configuration _configuration;

        public UdpSender(Configuration configuration)
        {
            _configuration = configuration;
            _udpClient = new UdpClient();
        }

        public async Task SendMessageAsync(Message message, CancellationToken cancellationToken = default)
        {
            byte[] messageBytes = Encoding.ASCII.GetBytes(message.Content);

            // UdpClient.SendAsync doesn't accept a cancellation token directly,
            // but we respect cancellation manually
            if (cancellationToken.IsCancellationRequested)
                return;

            await _udpClient.SendAsync(messageBytes, messageBytes.Length, _configuration.IpAddress, _configuration.Port)
                .ConfigureAwait(false);
        }

        public void Dispose()
        {
            _udpClient.Dispose();
        }
    }
}
