using CoreLibrary.Interfaces;
using CoreLibrary.Messaging.MessageTypes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoreLibrary.Messaging
{
    public class MessageTypeConverter : JsonConverter<IMessageType>
    {
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

        public override void Write(Utf8JsonWriter writer, IMessageType value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}