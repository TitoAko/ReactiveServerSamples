using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Messaging.MessageTypes;
using System.Net;
using System.Net.Sockets;

public class UdpCommunicator : ICommunicator
{
    private readonly UdpClient _udpClient;
    private readonly int _port;
    private readonly string _ipAddress;
    private readonly IMessageProcessor _messageProcessor;

    // Constructor to initialize UDP communicator with server IP and port
    public UdpCommunicator(string ipAddress, int port, IMessageProcessor messageProcessor)
    {
        _ipAddress = ipAddress;
        _port = port;
        _udpClient = new UdpClient(_ipAddress, _port);
        _messageProcessor = messageProcessor;
    }

    public void StartListening()
    {
        Console.WriteLine("UDP server is listening...");

        while (true)
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);  // Create a new endpoint
            var receivedBytes = _udpClient.Receive(ref endPoint);  // Pass by reference
            var message = ReceiveMessage();

            // Delegate message processing
            if (message != null)
            {
                _messageProcessor.ProcessMessage(message);
            }
        }
    }

    public void SendMessage(Message message)
    {
        byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(message.Content); // Convert message content to bytes
        _udpClient.Send(messageBytes, messageBytes.Length);  // Send message over UDP
    }

    public Message ReceiveMessage()
    {
        var endPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
        var receivedBytes = _udpClient.Receive(ref endPoint);  // Receive data
        string messageContent = System.Text.Encoding.ASCII.GetString(receivedBytes);

        IMessageType messageType = new TextMessage();  // Assume it's a text message
        return new Message("Client", messageContent, messageType);
    }
}