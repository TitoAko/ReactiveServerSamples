// CoreLibrary/IO/OutputHandler.cs
using CoreLibrary.Messaging;

namespace CoreLibrary.IO
{
    public sealed class OutputHandler
    {
        private const int MaxVisible = 240; // chars of message content

        public void DisplayMessage(Message message)
        {
            if (string.IsNullOrWhiteSpace(message.Content))
            {
                return;                              // <-- new: ignore empty lines
            }

            string content = message.Content;
            if (content.Length > MaxVisible)         // <-- new: truncate
            {
                content = content.Substring(0, MaxVisible) + "…";
            }

            if (message.Type == MessageType.Chat)
            {
                Console.WriteLine($"[{message.SentAt:HH:mm}] {message.Sender}: {content}");
            }
            else if (message.Type == MessageType.Exit)
            {
                Console.WriteLine($"*** {message.Sender} left ***");
            }
        }
    }
}
