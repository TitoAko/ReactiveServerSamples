using CoreLibrary.Interfaces;

namespace CoreLibrary.Messaging.MessageTypes
{
    /// <summary>
    /// Represents a message type used for system-level messages.
    /// </summary>
    public class SystemMessage : IMessageType
    {
        /// <summary>
        /// Processes a system message by displaying it with special formatting.
        /// </summary>
        /// <param name="sender">The sender of the system message.</param>
        /// <param name="content">The system message content.</param>
        public void ProcessMessage(string sender, string content)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[SYSTEM] {sender}: {content}");
            Console.ResetColor();
        }
    }
}
