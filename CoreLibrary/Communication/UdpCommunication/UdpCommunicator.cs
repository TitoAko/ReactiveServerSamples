using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Messaging.MessageTypes;
using CoreLibrary.Utilities;

namespace CoreLibrary.Communication.UdpCommunication
{
    /// <summary>
    /// Represents the main entry point for UDP-based communication.
    /// It coordinates sending and receiving messages via <see cref="UdpSender"/> and <see cref="UdpReceiver"/>.
    /// </summary>
    public class UdpCommunicator : ICommunicator
    {
        private readonly UdpReceiver _udpReceiver;
        private readonly UdpSender _udpSender;
        private readonly string _userName;

        /// <summary>
        /// Occurs when a message is received and parsed from the network.
        /// </summary>
        public event Action<Message>? OnMessageReceived;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpCommunicator"/> class.
        /// </summary>
        /// <param name="configuration">The configuration containing IP, port, and username settings.</param>
        public UdpCommunicator(Configuration configuration)
        {
            _udpReceiver = new UdpReceiver(configuration);
            _udpSender = new UdpSender(configuration);

            _udpReceiver.OnMessageReceived += RaiseMessageReceivedEvent;
            _userName = configuration.Username;
        }

        /// <summary>
        /// Initiates the listening operation and sends a connect message asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the listening task.</param>
        public void Connect(CancellationToken cancellationToken)
        {
            // Start the UDP receiver to listen for incoming messages
            var listenTask = Task.Run(() => StartListening(cancellationToken));
            // Optionally, send a connection message upon connection. Can be removed or handled as a notification to a server or remote clients.
            var connectMessage = new Message(_userName, "Connecting to server", new TextMessage());
            _ = SendMessage(connectMessage, cancellationToken); // fire and forget connect message
        }

        /// <summary>
        /// Starts the UDP receiver's observable stream to listen for incoming messages.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the listening stream.</param>
        public void StartListening(CancellationToken cancellationToken)
        {
            _udpReceiver.StartObservables(cancellationToken);
        }

        /// <summary>
        /// Sends a structured message asynchronously using UDP.
        /// </summary>
        /// <param name="message">The message object to be sent.</param>
        /// <param name="cancellationToken">Optional cancellation token for the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendMessage(Message message, CancellationToken cancellationToken = default)
        {
            await _udpSender.SendMessageAsync(message, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a placeholder test message for compatibility or testing purposes.
        /// </summary>
        /// <returns>A basic message instance.</returns>
        public Message ReceiveMessage()
        {
            return _udpReceiver.ReceiveMessage();
        }

        /// <summary>
        /// Handles raising the message event to listeners.
        /// </summary>
        /// <param name="message">The message received from the network.</param>
        private void RaiseMessageReceivedEvent(Message message)
        {
            // Raise the OnMessageReceived event for the ChatClient to handle
            OnMessageReceived?.Invoke(message);
        }

        /// <summary>
        /// Releases all managed resources used by the communicator,
        /// including sender, receiver, and event subscriptions.
        /// </summary>
        public void Dispose()
        {
            _udpReceiver.OnMessageReceived -= RaiseMessageReceivedEvent;
            _udpReceiver.Dispose();
            _udpSender.Dispose();
        }

        /// <summary>
        /// Stops the receiver and unsubscribes from all active observable streams.
        /// </summary>
        public void Stop()
        {
            _udpReceiver.Stop();
            // No Stop() on sender, but maybe log or extend later
        }
    }
}