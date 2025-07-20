using CoreLibrary.Interfaces;
using CoreLibrary.Messaging.MessageTypes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoreLibrary.Messaging
{
    /// <summary>
    /// JSON converter for polymorphic deserialization of <see cref="IMessageType"/>.
    /// This enables handling of different message types (e.g., Text, Typing, Disconnect) during deserialization.
    /// </summary>
    public class MessageTypeConverter : JsonConverter<IMessageType>
    {
        /// <summary>
        /// Reads and deserializes an <see cref="IMessageType"/> from a JSON payload.
        /// This method looks for the "Type" discriminator property to determine which concrete class to deserialize.
        /// </summary>
        /// <param name="reader">The JSON reader.</param>
        /// <param name="typeToConvert">The type to convert (ignored here, always <see cref="IMessageType"/>).</param>
        /// <param name="options">Serializer options including custom converters.</param>
        /// <returns>The deserialized <see cref="IMessageType"/> instance.</returns>
        /// <exception cref="JsonException">Thrown if the type is missing or unknown.</exception>
        public override IMessageType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            if (!doc.RootElement.TryGetProperty("Type", out var typeProperty))
                throw new JsonException("Missing discriminator property 'Type'.");

            var typeValue = typeProperty.GetString();

            return typeValue?.ToLower() switch
            {
                "text" => JsonSerializer.Deserialize<TextMessage>(doc.RootElement.GetRawText(), options),
                "typing" => JsonSerializer.Deserialize<TypingMessage>(doc.RootElement.GetRawText(), options),
                "disconnect" => JsonSerializer.Deserialize<DisconnectMessage>(doc.RootElement.GetRawText(), options),
                "system" => JsonSerializer.Deserialize<SystemMessage>(doc.RootElement.GetRawText(), options),
                _ => throw new JsonException($"Unknown message type: {typeValue}")
            };
        }

        /// <summary>
        /// Writes an <see cref="IMessageType"/> to JSON using the appropriate concrete type.
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="value">The <see cref="IMessageType"/> instance to serialize.</param>
        /// <param name="options">The serializer options to apply.</param>
        public override void Write(Utf8JsonWriter writer, IMessageType value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}