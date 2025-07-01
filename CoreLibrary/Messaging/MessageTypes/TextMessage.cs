using CoreLibrary.Interfaces;

namespace CoreLibrary.Messaging.MessageTypes
{
    public class TextMessage : IMessageType
    {
        public void ProcessMessage(string sender, string content)
        {
            // Process the text message
            Console.WriteLine($"Processing text message from {sender}: {content}");
        }
    }
}
