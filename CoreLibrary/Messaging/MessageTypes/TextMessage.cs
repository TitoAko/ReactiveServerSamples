using CoreLibrary.Interfaces;

namespace CoreLibrary.Messaging.MessageTypes
{
    /// <summary>
    /// Represents a plain text message.
    /// </summary>
    public class TextMessage : IMessageType
    {
        /// <summary>
        /// Gets the type identifier of this message.
        /// </summary>
        public string Type => "text";

        /// <summary>
        /// Displays the text message in a simple formatted output.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="content">The content of the message.</param>
        public void ProcessMessage(string sender, string content)
        {
            Console.WriteLine($"{sender}: {content}");
        }
    }
}
