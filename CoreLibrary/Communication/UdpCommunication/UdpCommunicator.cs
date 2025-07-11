using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Messaging.MessageTypes;
using System.Net;
using System.Net.Sockets;

namespace CoreLibrary.Communication.UdpCommunication
{
    public class UdpCommunicator : ICommunicator
    {
        private readonly UdpClient _udpClient;
        private readonly int _port;
        private readonly string _ipAddress;
        private readonly string _userName;
        private IPEndPoint _endPoint;

        // Constructor to initialize UDP communicator with server IP and port
        public UdpCommunicator(UdpClient udpClient, string ipAddress, int port, string userName)
        {
            _udpClient = udpClient;
            _ipAddress = ipAddress;
            _port = port;
            _userName = userName;
            _endPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
        }

        // Constructor to initialize UDP communicator with client IP and port
        public UdpCommunicator(string ipAddress, int port, string userName)
            : this(new UdpClient(), ipAddress, port, userName)
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
                if (_udpClient.Client.Connected)
                {
                    if (_udpClient.Available > 0)
                    {
                        Console.WriteLine("Checking for messages, 5 sec interval...");
                        try
                        {
                            var message = ReceiveMessage();  // Receive message from the client
                            if (message != null)
                            {
                                Console.WriteLine($"Received message: {message.Content} from {message.Sender}");
                                // Process the received message (e.g., display it, log it, etc.)
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error receiving message: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No messages available, waiting for 5 seconds...");
                        Thread.Sleep(5000);  // Wait for 5 seconds before checking again
                    }
                }
                else
                {
                    Console.WriteLine("UDP client is not connected. Can't receive messages");
                }
            }
        }

        public void SendMessage(Message message)
        {
            Console.WriteLine($"Sending message: {message.Content} to {_ipAddress}:{_port}");
            byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(message.Content); // Convert message content to bytes
            try
            {
                if (_udpClient.Client.Connected)
                {
                    _udpClient.Send(messageBytes, messageBytes.Length);  // Send message over UDP
                    Console.WriteLine($"Message sent: {message.Content}");
                }
                else
                {
                    Console.WriteLine("UDP client is not connected. Cannot send message.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Message> ReceiveMessageAsync()
        {
            var receivedBytes = await _udpClient.ReceiveAsync();  // Receive data asynchronously
            string messageContent = System.Text.Encoding.ASCII.GetString(receivedBytes.Buffer);
            IMessageType messageType = new TextMessage();  // Assume it's a text message
            return new Message(_userName, messageContent, messageType);
        }

        public Message ReceiveMessage()
        {
            _udpClient.Client.ReceiveTimeout = 5000; // Set a timeout for receiving messages
            byte[] receiveData = _udpClient.Receive(ref _endPoint);  // Receive data synchronously
            string messageContent = System.Text.Encoding.ASCII.GetString(receiveData);
            IMessageType messageType = new TextMessage();  // Assume it's a text message
            var message = new Message(_userName, messageContent, messageType);
            // var message = Task.Run(() => ReceiveMessageAsync());
            return message;
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
            if (_udpClient != null && !_udpClient.Client.Connected)
            {
                Console.WriteLine("Connecting to the UDP server...");
                _udpClient.Connect(_endPoint);  // Connect the UDP client to the endpoint
            }
            else
            {
                Console.WriteLine("UDP client is already connected or not initialized.");
            }
        }
    }
}