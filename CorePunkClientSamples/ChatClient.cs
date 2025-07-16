using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Handlers;
using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;

namespace ClientApp
{
    public class ChatClient : IClient
    {
        private readonly ClientHandler _clientHandler;  // Reference to ClientHandler
        private readonly OutputHandler _outputHandler;
        private readonly ICommunicator _communicator;  // This is the UdpCommunicator now
        private readonly string _username;
        private CancellationTokenSource _cancellationTokenSource;

        public ChatClient(ClientHandler clientHandler, OutputHandler outputHandler, ICommunicator communicator, string username)
        {
            _clientHandler = clientHandler;
            _outputHandler = outputHandler;
            _communicator = communicator;
            _username = username;
            _cancellationTokenSource = new CancellationTokenSource();

            // Subscribe to events raised by ClientHandler
            _clientHandler.OnMessageReceived += DisplayReceivedMessage;
            _clientHandler.OnConnect += Connect;
            _clientHandler.OnDisconnect += Disconnect;

            // Subscribe to the message receiver (UdpReceiver inside UdpCommunicator)
            if (_communicator is UdpCommunicator udpCommunicator)
            {
                udpCommunicator.OnMessageReceived += HandleReceivedMessage;
            }
        }

        // Implement IClient methods
        public void SendMessage(Message message)
        {
            _communicator.SendMessage(message);  // Send message through communicator
        }

        public Message ReceiveMessage()
        {
            return _communicator.ReceiveMessage();  // Receive message through communicator
        }

        public void DisplayReceivedMessage(Message message)
        {
            _outputHandler.DisplayMessage(message.Sender, message.Content);
        }

        public void HandleReceivedMessage(Message message)
        {
            // Process incoming messages
            _clientHandler.ReceiveMessage(message);
        }

        public void Connect()
        {
            Console.WriteLine("Connecting to the server...");
            _communicator.Connect(_cancellationTokenSource.Token);  // Start listening for incoming messages
            Console.WriteLine("Connected to the server");
        }

        public void Disconnect()
        {
            Console.WriteLine("Disconnecting from the server...");
            _communicator.Stop();  // Stop listening for messages
            Console.WriteLine("Disconnected from the server");
        }

        public void StartInputHandler()
        {
            InputHandler inputHandler = new InputHandler(this, _username);
            Task.Run(() => inputHandler.HandleUserInput());  // Run user input handler on a new task
        }

        public void Dispose()
        {
            _clientHandler.OnMessageReceived -= DisplayReceivedMessage;
            _clientHandler.OnConnect -= Connect;
            _clientHandler.OnDisconnect -= Disconnect;
            _communicator.Dispose();
        }
    }
}
