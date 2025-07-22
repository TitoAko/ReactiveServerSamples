using CoreLibrary.IO;
using CoreLibrary.Messaging;

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
            Assert.Equal(string.Empty, writer.ToString());
        }

        [Fact]
        public void VeryLongLine_Truncates()
        {
            var longMsg = new string('y', 300);
            var handler = new OutputHandler();

            var writer = new StringWriter();
            Console.SetOut(writer);

            handler.DisplayMessage(new Message("alice", longMsg, MessageType.Chat));

            string output = writer.ToString();
            Assert.True(output.Length < 260); // displays HH:mm + sender + ~240 chars

            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }
    }
}
