using CoreLibrary.Interfaces;

namespace CoreLibrary.Messaging
{
    public class Message
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public IMessageType MessageType { get; set; }

        public Message(string sender, string content, IMessageType messageType)
        {
            Sender = sender;
            Content = content;
            MessageType = messageType;
        }
        public void Process()
        {
            // Forward the data to the appropriate IMessageType
            MessageType.ProcessMessage(Sender, Content);
        }
    }
}
