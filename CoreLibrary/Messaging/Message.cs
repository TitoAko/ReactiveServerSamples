using CoreLibrary.Interfaces;
using CoreLibrary.Messaging.MessageTypes;
using System.Text.Json.Serialization;

namespace CoreLibrary.Messaging
{
    /// <summary>
    /// Represents a structured chat message containing sender, content, and type.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets the username of the message sender.
        /// </summary>
        public string Sender { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the message type (text, typing, disconnect, etc.).
        /// </summary>
        [JsonConverter(typeof(MessageTypeConverter))]
        public IMessageType MessageType { get; set; } = new TextMessage();

        /// <summary>
        /// Initializes a new instance of <see cref="Message"/> with all parameters.
        /// </summary>
        public Message(string sender, string content, IMessageType messageType)
        {
            Sender = sender;
            Content = content;
            MessageType = messageType;
        }

        /// <summary>
        /// Parameterless constructor for deserialization.
        /// </summary>
        public Message() { } // For deserialization

        /// <summary>
        /// Delegates message-specific processing to the appropriate <see cref="IMessageType"/>.
        /// </summary>
        public void Process()
        {
            // Forward the data to the appropriate IMessageType
            MessageType?.ProcessMessage(Sender, Content);
        }
    }
}
