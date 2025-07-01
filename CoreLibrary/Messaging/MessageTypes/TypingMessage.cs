using CoreLibrary.Interfaces;

namespace CoreLibrary.Messaging.MessageTypes
{
    internal class TypingMessage : IMessageType
    {
        public void ProcessMessage(string sender, string content)
        {
            // Process typing indicator
            Console.WriteLine($"{sender} is typing...");
        }
    }
}
