using CoreLibrary.Interfaces;
using CoreLibrary.Messaging.MessageTypes;
using System.Text.Json.Serialization;

namespace CoreLibrary.Messaging
{
    public class Message
    {
        public string Sender { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        [JsonConverter(typeof(MessageTypeConverter))]
        public IMessageType MessageType { get; set; } = new TextMessage();

        public Message(string sender, string content, IMessageType messageType)
        {
            Sender = sender;
            Content = content;
            MessageType = messageType;
        }

        public Message() { } // For deserialization

        public void Process()
        {
            // Forward the data to the appropriate IMessageType
            MessageType?.ProcessMessage(Sender, Content);
        }
    }
}
