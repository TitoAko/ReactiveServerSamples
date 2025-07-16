using CoreLibrary.Messaging;
using CoreLibrary.Utilities;
using System.Net.Sockets;

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

        public void SendMessage(Message message)
        {
            byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(message.Content); // Convert message content to bytes
            _udpClient.Send(messageBytes, messageBytes.Length, _configuration.IpAddress, _configuration.Port);  // Send message over UDP
        }

        public void Dispose()
        {
            _udpClient.Dispose();
        }
    }
}
