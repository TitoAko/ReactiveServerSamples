using CoreLibrary.Handlers;
using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Messaging.MessageTypes;
using CoreLibrary.Utilities;

namespace ClientApp
{
    public class ChatClient : IClient
    {
        private readonly ClientHandler _clientHandler;  // Reference to ClientHandler
        private readonly OutputHandler _outputHandler;
        private readonly Configuration _config;

        public ChatClient(ClientHandler clientHandler, OutputHandler outputHandler, Configuration config)
        {
            _clientHandler = clientHandler;
            _outputHandler = outputHandler;
            _config = config;

            // Subscribe to events raised by ClientHandler
            _clientHandler.OnMessageReceived += DisplayReceivedMessage;
            _clientHandler.OnConnect += Connect;
            _clientHandler.OnDisconnect += Disconnect;
        }

        // Implement IClient methods
        public void SendMessage(Message message)
        {
            // Logic to send message
            // For example, use a communicator to send the message
        }

        public Message ReceiveMessage()
        {
            // Logic to receive a message
            // For example, listen to incoming messages
            return new Message("Server", "Sample message", new TextMessage());
        }

        public void DisplayReceivedMessage(Message message)
        {
            _outputHandler.DisplayMessage(message.Sender, message.Content);
        }

        public void Connect()
        {
            Console.WriteLine("Connected to the server");
        }

        public void Disconnect()
        {
            Console.WriteLine("Disconnected from the server");
        }

        public void StartListening()
        {
            // Start listening for messages, etc.
        }
    }
}
