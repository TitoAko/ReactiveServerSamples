using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoreLibrary.Messaging
{
    /// <summary>Serialises enum as lowercase strings ("chat", "exit").</summary>
    public class MessageTypeConverter : JsonConverter<MessageType>
    {
        public override MessageType Read(ref Utf8JsonReader reader,
                                         Type typeToConvert,
                                         JsonSerializerOptions options)
            => Enum.Parse<MessageType>(reader.GetString()!, ignoreCase: true);

        public override void Write(Utf8JsonWriter writer,
                                   MessageType value,
                                   JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString().ToLowerInvariant());
    }
}
