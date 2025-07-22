using CoreLibrary.Messaging;
using System.Text.Json;

namespace CoreLibrary.Tests.Messaging
{
    public class MessageSerializationTests
    {
        private readonly JsonSerializerOptions _opts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new MessageTypeConverter() }
        };

        [Fact]
        public void RoundTrip_PreservesAllFields()
        {
            var original = new Message("alice", "hi there", MessageType.Chat);

            string json = JsonSerializer.Serialize(original, _opts);
            var backAgain = JsonSerializer.Deserialize<Message>(json, _opts)!;

            Assert.Equal(original.Content, backAgain.Content);
            Assert.Equal(original.Sender, backAgain.Sender);
            Assert.Equal(original.Type, backAgain.Type);
        }

        [Fact]
        public void Converter_UsesLowercase_Strings()
        {
            string json = JsonSerializer.Serialize(
                new Message("bob", "ping", MessageType.Exit), _opts);

            Assert.Contains("\"type\":\"exit\"", json);
        }
    }
}
