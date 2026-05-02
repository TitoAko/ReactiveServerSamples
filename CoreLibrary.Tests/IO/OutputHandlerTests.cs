using CoreLibrary.IO;
using CoreLibrary.Messaging;

namespace CoreLibrary.Tests.IO
{
    public class OutputHandlerTests
    {
        [Fact]
        public void DisplayMessage_Chat_WritesSenderAndContent()
        {
            TextWriter originalOut = Console.Out;
            var writer = new StringWriter();

            try
            {
                Console.SetOut(writer);

                var handler = new OutputHandler();
                handler.DisplayMessage(new Message("alice", "hello", MessageType.Chat));

                string output = writer.ToString();

                Assert.Contains("[", output);
                Assert.Contains("alice", output);
                Assert.Contains("hello", output);
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Fact]
        public void DisplayMessage_Exit_WritesSenderLeftMessage()
        {
            TextWriter originalOut = Console.Out;
            var writer = new StringWriter();

            try
            {
                Console.SetOut(writer);

                var handler = new OutputHandler();
                handler.DisplayMessage(new Message("bob", "<left>", MessageType.Exit));

                string output = writer.ToString();

                Assert.Contains("***", output);
                Assert.Contains("bob", output);
                Assert.Contains("left", output);
                Assert.DoesNotContain("<left>", output);
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
    }
}