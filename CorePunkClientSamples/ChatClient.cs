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
        private readonly ICommunicator _communicator;
        private readonly Configuration _config;
        private readonly InputHandler _inputHandler;
        private CancellationTokenSource _cts;  // Cancellation token for graceful shutdown

        public ChatClient(ClientHandler clientHandler, OutputHandler outputHandler, ICommunicator communicator, Configuration config)
        {
            _clientHandler = clientHandler;
            _outputHandler = outputHandler;
            _communicator = communicator;

            // Subscribe to events raised by ClientHandler
            _clientHandler.OnMessageReceived += DisplayReceivedMessage;
            _clientHandler.OnConnect += Connect;
            _clientHandler.OnDisconnect += Disconnect;
            _config = config;


            _inputHandler = new InputHandler(this, _config.Username);

            _cts = new CancellationTokenSource();
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
            Task.Run(StartListening); // Start listening for incoming messages
            Task.Run(StartInputHandler); // Start handling user input
        }

        // Start listening in a separate task (non-blocking)
        public void StartListening()
        {
            _communicator.Connect();  // Ensure the communicator is connected before starting to listen
            _communicator.StartListening();
            Console.WriteLine("Listening for incoming messages finished...");
        }

        public void StartInputHandler()
        {
            _inputHandler.HandleUserInput();
        }

        public void Disconnect()
        {
            Console.WriteLine("Disconnecting from the server...");
            _communicator.StopListening();  // Stop the listening process in the communicator

            Console.WriteLine("Disconnected from the server");
            _cts?.Cancel();  // Cancel the token to stop listening and input handling
        }

        public void Dispose()
        {
            _cts?.Cancel();  // Cancel the token to stop listening and input handling
            _communicator.Dispose();  // Dispose of the communicator
            _clientHandler.OnMessageReceived -= DisplayReceivedMessage;  // Unsubscribe from events
            _clientHandler.OnConnect -= Connect;
            _clientHandler.OnDisconnect -= Disconnect;
        }
    }
}
