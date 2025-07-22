// CoreLibrary/IO/OutputHandler.cs
using CoreLibrary.Messaging;

namespace CoreLibrary.IO
{
    public sealed class OutputHandler
    {
        private const int MaxVisible = 240; // chars of message content

        public void DisplayMessage(Message m)
        {
            if (string.IsNullOrWhiteSpace(m.Content))
                return;                              // <-- new: ignore empty lines

            string content = m.Content;
            if (content.Length > MaxVisible)         // <-- new: truncate
                content = content.Substring(0, MaxVisible) + "…";

            if (m.Type == MessageType.Chat)
                Console.WriteLine($"[{m.SentAt:HH:mm}] {m.Sender}: {content}");
            else if (m.Type == MessageType.Exit)
                Console.WriteLine($"*** {m.Sender} left ***");
        }
    }
}
