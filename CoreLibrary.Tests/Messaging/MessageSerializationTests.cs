using System.Text.Json;

using CoreLibrary.Messaging;

namespace CoreLibrary.Tests.Messaging
{
    public class MessageSerializationTests
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new MessageTypeConverter() }
        };

        [Fact]
        public void RoundTrip_PreservesAllFields()
        {
            var original = new Message("alice", "hi there", MessageType.Chat);

            string jsonSerializer = JsonSerializer.Serialize(original, _jsonSerializerOptions);
            var backAgain = JsonSerializer.Deserialize<Message>(jsonSerializer, _jsonSerializerOptions)!;

            Assert.Equal(original.Content, backAgain.Content);
            Assert.Equal(original.Sender, backAgain.Sender);
            Assert.Equal(original.Type, backAgain.Type);
        }

        [Fact]
        public void Converter_UsesLowercase_Strings()
        {
            string jsonSerializer = JsonSerializer.Serialize(
                new Message("bob", "ping", MessageType.Exit), _jsonSerializerOptions);

            Assert.Contains("\"type\":\"exit\"", jsonSerializer);
        }
    }
}
