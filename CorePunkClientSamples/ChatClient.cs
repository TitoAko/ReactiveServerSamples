using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Handlers;
using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;

namespace ClientApp
{
    /// <summary>
    /// Represents the main client class that coordinates sending, receiving, and displaying messages.
    /// </summary>
    public class ChatClient : IClient
    {
        private readonly ClientHandler _clientHandler;  // Reference to ClientHandler
        private readonly OutputHandler _outputHandler;
        private readonly ICommunicator _communicator;  // This is the UdpCommunicator now
        private readonly string _username;
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatClient"/> class.
        /// </summary>
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

        /// <summary>
        /// Sends a message to the server using the configured communicator.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void SendMessage(Message message)
        {
            _communicator.SendMessage(message);  // Send message through communicator
        }

        /// <summary>
        /// Receives a message from the server through the configured communicator.
        /// </summary>
        /// <returns>The received message.</returns>
        public Message ReceiveMessage()
        {
            return _communicator.ReceiveMessage();  // Receive message through communicator
        }

        /// <summary>
        /// Displays the received message using the configured output handler.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public void DisplayReceivedMessage(Message message)
        {
            _outputHandler.DisplayMessage(message.Sender, message.Content);
        }

        /// <summary>
        /// Handles messages received from the communicator and forwards them to the client handler.
        /// </summary>
        public void HandleReceivedMessage(Message message)
        {
            // Process incoming messages
            _clientHandler.ReceiveMessage(message);
        }

        /// <summary>
        /// Initiates the connection to the server and starts listening for messages.
        /// </summary>
        public void Connect()
        {
            Console.WriteLine("Connecting to the server...");
            _communicator.Connect(_cancellationTokenSource.Token);  // Start listening for incoming messages
            Console.WriteLine("Connected to the server");
        }

        /// <summary>
        /// Disconnects from the server and stops listening for messages.
        /// </summary>
        public void Disconnect()
        {
            Console.WriteLine("Disconnecting from the server...");
            _communicator.Stop();  // Stop listening for messages
            Console.WriteLine("Disconnected from the server");
        }

        /// <summary>
        /// Starts the user input loop in a background task.
        /// </summary>
        public void StartInputHandler()
        {
            InputHandler inputHandler = new InputHandler(this, _username);
            Task.Run(() => inputHandler.HandleUserInput());  // Run user input handler on a new task
        }

        /// <summary>
        /// Unsubscribes from events and releases resources used by the chat client.
        /// </summary>
        public void Dispose()
        {
            _clientHandler.OnMessageReceived -= DisplayReceivedMessage;
            _clientHandler.OnConnect -= Connect;
            _clientHandler.OnDisconnect -= Disconnect;
            _communicator.Dispose();
        }
    }
}
