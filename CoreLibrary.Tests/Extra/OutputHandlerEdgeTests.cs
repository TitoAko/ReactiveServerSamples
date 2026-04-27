using CoreLibrary.IO;
using CoreLibrary.Messaging;

using FluentAssertions;

namespace CoreLibrary.Tests.Extra
{

    public class OutputHandlerEdgeTests
    {
        [Fact]
        public void EmptyMessage_PrintsNothing()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            var handler = new OutputHandler();
            handler.DisplayMessage(new Message("bob", "", MessageType.Chat));

            Console.Out.Flush();
            writer.ToString().Trim().Should().BeEmpty();
        }

        [Fact]
        public void VeryLongLine_Truncates()
        {
            var longMessage = new string('y', 300);
            var outputHandler = new OutputHandler();

            var writer = new StringWriter();
            var original = Console.Out;
            try
            {
                Console.SetOut(writer);

                outputHandler.DisplayMessage(new Message("alice", longMessage, MessageType.Chat));

                string output = writer.ToString();
                output.Length.Should().BeLessThanOrEqualTo(259); // displays HH:mm + sender + ~240 chars
            }
            finally
            {
                Console.SetOut(original);
            }
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }
    }
}
