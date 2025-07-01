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
        public void SendMessage(Message message, ICommunicator communicator)
        {
            // Logic to send message
            communicator.SendMessage(message);
        }

        public Message ReceiveMessage(ICommunicator communicator)
        {
            // Logic to receive a message
            return communicator.ReceiveMessage();
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
