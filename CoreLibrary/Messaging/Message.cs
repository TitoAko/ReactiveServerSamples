using System.Text.Json.Serialization;

namespace CoreLibrary.Messaging
{
    /// <summary>Small, closed set of message kinds understood by both client & server.</summary>
    public enum MessageType
    {
        Chat,
        Exit
    }

    /// <summary>POD‑style DTO; add fields cautiously to keep packets tiny.</summary>
    public class Message
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Sender { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; } = MessageType.Chat;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // Convenience ctor for old call‑sites that did 3‑arg new Message(...)
        [JsonConstructor]                       // keeps deserialiser happy
        public Message() { }

        public Message(string sender, string content, MessageType type = MessageType.Chat)
        {
            Sender = sender;
            Content = content;
            Type = type;
        }
    }
}
