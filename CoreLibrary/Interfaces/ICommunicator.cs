using CoreLibrary.Messaging;

namespace CoreLibrary.Interfaces
{
    /// <summary>
    /// Defines a standard communication contract for network-based message exchange.
    /// </summary>
    public interface ICommunicator : IDisposable
    {
        /// <summary>
        /// Sends a structured message to the connected target.
        /// </summary>
        /// <param name="message">The message object containing sender, content, and type.</param>
        Task SendMessage(Message message, CancellationToken cancellationToken = default);
        /// <summary>
        /// Receives a message from the connected source.
        /// </summary>
        /// <returns>A Message instance representing the received data.</returns>
        Message ReceiveMessage();
        /// <summary>
        /// Begins listening for incoming messages using the provided cancellation token.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the listening operation.</param>
        void StartListening(CancellationToken cancellationToken);
        /// <summary>
        /// Stops any ongoing listening or communication activity.
        /// </summary>
        void Stop();
        /// <summary>
        /// Establishes a connection and optionally performs a handshake or introduction message.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the connection attempt.</param>
        void Connect(CancellationToken cancellationToken);
    }
}
