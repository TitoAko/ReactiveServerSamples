using CoreLibrary.Messaging;

namespace CoreLibrary.Interfaces
{
    /// <summary>
    /// Transport‑agnostic contract for chat traffic.
    /// </summary>
    public interface ICommunicator : IDisposable
    {
        /// <summary>
        /// Sends <paramref name="message"/> asynchronously.
        /// </summary>
        /// <param name="message">Message that's being sent</param>
        /// <param name="cancellationToken">Optional, token to end waiting for messages to be sent.</param>
        /// <returns></returns>
        Task SendMessageAsync(Message message,
                              CancellationToken cancellationToken = default);

        /// <summary>Push stream of inbound messages.</summary>
        event EventHandler<Message>? MessageReceived;

        /// <summary>
        /// Starts the background listen loop.
        /// </summary>
        /// <param name="cancellationToken">Optional, token to end the background listen loop.</param>
        Task StartAsync(CancellationToken cancellationToken = default);
    }
}