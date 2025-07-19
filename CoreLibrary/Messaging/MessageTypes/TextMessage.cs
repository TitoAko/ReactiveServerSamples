using CoreLibrary.Interfaces;

namespace CoreLibrary.Messaging.MessageTypes
{
    public class TextMessage : IMessageType
    {
        public string Type => "text";
        public void ProcessMessage(string sender, string content)
        {
            Console.WriteLine($"{sender}: {content}");
        }
    }
}
