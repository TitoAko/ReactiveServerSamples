using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Messaging.MessageTypes;
using System.Net;
using System.Net.Sockets;

namespace CoreLibrary.Communication
{
    public class TcpCommunicator : ICommunicator
    {
        private readonly TcpListener _tcpListener;
        private readonly int _port;
        private readonly string _ipAddress;

        public TcpCommunicator(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
            _tcpListener = new TcpListener(IPAddress.Parse(_ipAddress), _port);
        }

        public void StartListening(IMessageProcessor messageProcessor)
        {
            _tcpListener.Start();
            Console.WriteLine("TCP server is listening...");

            while (true)
            {
                var message = ReceiveMessage();     // Receive message from the client

                if (message != null)
                {
                    // Process the received message
                    messageProcessor.ProcessMessage(message);
                }
            }
        }

        public void SendMessage(Message message)
        {
            using (var tcpClient = new TcpClient(_ipAddress, _port))
            {
                var stream = tcpClient.GetStream();
                byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(message.Content); // Convert message content to bytes
                stream.Write(messageBytes, 0, messageBytes.Length);  // Send message
            }
        }

        public Message ReceiveMessage()
        {
            // For demonstration, we are receiving a text message from a TCP client
            var tcpClient = _tcpListener.AcceptTcpClient();
            var stream = tcpClient.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string messageContent = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);

            // You can now process the message based on its type
            IMessageType messageType = new TextMessage();  // We assume it's a text message for this example
            return new Message("Client", messageContent, messageType);
        }

        public void StopListening()
        {
            _tcpListener.Stop();  // Stop listening for incoming messages
        }

        public void Dispose()
        {
            _tcpListener.Dispose();
        }
    }
}
