using CoreLibrary.Interfaces;

namespace CoreLibrary.Messaging.MessageTypes
{
    /// <summary>
    /// Represents a typing notification message.
    /// </summary>
    internal class TypingMessage : IMessageType
    {
        /// <summary>
        /// Handles the typing message by printing a typing indicator.
        /// </summary>
        /// <param name="sender">The sender of the typing status.</param>
        /// <param name="content">Additional context or preview.</param>
        public void ProcessMessage(string sender, string content)
        {
            Console.WriteLine($"{sender} is typing...");
            Console.WriteLine($"{content}");
        }
    }
}
