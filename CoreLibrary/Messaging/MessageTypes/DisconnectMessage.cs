using CoreLibrary.Interfaces;

namespace CoreLibrary.Messaging.MessageTypes
{
    public class DisconnectMessage : IMessageType
    {
        public void ProcessMessage(string sender, string content)
        {
            Console.WriteLine($"Message {sender} has disconnected");
        }
    }
}