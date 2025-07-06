using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Messaging.MessageTypes;
using System.Net;
using System.Net.Sockets;

namespace CoreLibrary.Communication
{
    public class UdpCommunicator : ICommunicator
    {
        private readonly UdpClient _udpClient;
        private readonly int _port;
        private readonly string _ipAddress;
        private readonly string _userName;
        private IPEndPoint _endPoint;

        // Constructor to initialize UDP communicator with server IP and port
        public UdpCommunicator(string ipAddress, int port, string userName)
        {
            _ipAddress = ipAddress;
            _port = port;
            _udpClient = new UdpClient(_port);
            _userName = userName;
        }

        public void StartListening()
        {
            Console.WriteLine("UDP server is listening...");

            while (true)
            {
                var message = ReceiveMessage();
            }
        }

        public void SendMessage(Message message)
        {
            Console.WriteLine($"Sending message: {message.Content} to {_ipAddress}:{_port}");
            byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(message.Content); // Convert message content to bytes
            _udpClient.Send(messageBytes, messageBytes.Length);  // Send message over UDP
        }

        public Message ReceiveMessage()
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
            var receivedBytes = _udpClient.Receive(ref endPoint);  // Receive data
            string messageContent = System.Text.Encoding.ASCII.GetString(receivedBytes);

            IMessageType messageType = new TextMessage();  // Assume it's a text message
            return new Message(_userName, messageContent, messageType);
        }

        /// <summary>
        /// Close the UDP client and stop listening for messages.
        /// </summary>
        public void StopListening()
        {
            _udpClient.Close();
        }

        public void Dispose()
        {
            _udpClient.Dispose();
        }

        public void Connect()
        {
            Console.WriteLine("Connecting to the UDP server...");
            // Resolve the IP address from the hostname (if needed)
            var hostAddresses = Dns.GetHostAddresses(_ipAddress);
            _endPoint = new IPEndPoint(hostAddresses[0], _port);  // Use the first address returned (for simplicity)
            Console.WriteLine($"UDP communicator initialized for {_userName} at {_ipAddress}:{_port}");

            _udpClient.Connect(_endPoint);  // Connect the UDP client to the endpoint

            // In UDP, we don't have a persistent connection like TCP, but we can send a message to establish communication
            //var connectMessage = new Message(_userName, "Connecting to server", new TextMessage());
            //SendMessage(connectMessage);
            Console.WriteLine($"Connected to the UDP server, {IPAddress.Parse(_ipAddress)}, {_port}");
        }
    }
}