using CoreLibrary.IO;
using CoreLibrary.Messaging;

namespace CoreLibrary.Tests.IO
{
    public class OutputHandlerTests
    {
        [Theory]
        [InlineData(MessageType.Chat, "hello", "alice", "[")]
        [InlineData(MessageType.Exit, "<left>", "bob", "***")]
        public void DisplayMessage_WritesExpectedLine(MessageType type, string content, string sender, string expectedToken)
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            var handler = new OutputHandler();
            handler.DisplayMessage(new Message(sender, content, type));

            Console.Out.Flush();
            string output = writer.ToString();
            Assert.Contains(expectedToken, output);

            // restore console
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }
    }
}
