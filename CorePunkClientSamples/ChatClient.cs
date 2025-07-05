using CoreLibrary.Handlers;
using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace ClientApp
{
    public class ChatClient : IClient
    {
        private readonly ClientHandler _clientHandler;  // Reference to ClientHandler
        private readonly OutputHandler _outputHandler;
        private readonly Configuration _config;
        private readonly ICommunicator _communicator;

        public ChatClient(ClientHandler clientHandler, OutputHandler outputHandler, Configuration config, ICommunicator communicator)
        {
            _clientHandler = clientHandler;
            _outputHandler = outputHandler;
            _config = config;
            _communicator = communicator;

            // Subscribe to events raised by ClientHandler
            _clientHandler.OnMessageReceived += DisplayReceivedMessage;
            _clientHandler.OnConnect += Connect;
            _clientHandler.OnDisconnect += Disconnect;
        }

        // Implement IClient methods
        public void SendMessage(Message message)
        {
            // Logic to send message
            _communicator.SendMessage(message);
        }

        public Message ReceiveMessage()
        {
            // Logic to receive a message
            return _communicator.ReceiveMessage();
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
