using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoreLibrary.Messaging
{
    /// <summary>
    /// Simple System.Text.Json wrapper that (de)serialises <see cref="Message"/> and
    /// preserves the concrete <see cref="Message.Type"/> via a type discriminator.
    /// </summary>
    public static class MessageSerializer
    {
        private const string TypeDiscriminator = "$type";

        private static readonly JsonSerializerOptions Options = new()
        {
            Converters = { new MessageJsonConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>Serialises to UTF-8 bytes.</summary>
        public static byte[] Serialize(Message msg)
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msg, Options));
        }

        /// <summary>Deserialises from UTF-8 bytes.</summary>
        public static Message Deserialize(ReadOnlySpan<byte> bytes)
        {
            return JsonSerializer.Deserialize<Message>(bytes, Options)!
                ?? throw new InvalidOperationException("Failed to deserialise Message.");
        }

        // ---------- nested converter ----------
        private sealed class MessageJsonConverter : JsonConverter<Message>
        {
            public override Message? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions opts)
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                if (!doc.RootElement.TryGetProperty(TypeDiscriminator, out var typeProp))
                {
                    throw new JsonException($"Missing {TypeDiscriminator} discriminator.");
                }

                var typeName = typeProp.GetString();
                var target = Type.GetType(typeName!, throwOnError: true)!;

                var json = doc.RootElement.GetRawText();
                return (Message)JsonSerializer.Deserialize(json, target, opts)!;
            }

            public override void Write(Utf8JsonWriter writer, Message value, JsonSerializerOptions opts)
            {
                using var doc = JsonDocument.Parse(JsonSerializer.Serialize(value, value.GetType(), opts));
                writer.WriteStartObject();

                // copy properties verbatim
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    prop.WriteTo(writer);
                }

                // add discriminator
                writer.WriteString(TypeDiscriminator, value.GetType().AssemblyQualifiedName);
                writer.WriteEndObject();
            }
        }
    }
}