// CoreLibrary/IO/OutputHandler.cs
using CoreLibrary.Messaging;

namespace CoreLibrary.IO
{
    public sealed class OutputHandler
    {
        public void DisplayMessage(Message m)
        {
            if (m.Type == MessageType.Chat)
                Console.WriteLine($"[{m.SentAt:HH:mm}] {m.Sender}: {m.Content}");
            else if (m.Type == MessageType.Exit)
                Console.WriteLine($"*** {m.Sender} left ***");
        }
    }
}
