using CoreLibrary.Messaging;
using CoreLibrary.Messaging.MessageTypes;
using CoreLibrary.Interfaces;
using Moq;

namespace CoreLibrary.Tests.Messaging
{
    public class MessageTests
    {
        [Fact]
        public void ShouldInitializeMessageWithCorrectProperties()
        {
            var msgType = new TextMessage();
            var message = new Message("Sender1", "Hello world", msgType);

            Assert.Equal("Sender1", message.Sender);
            Assert.Equal("Hello world", message.Content);
            Assert.Equal(msgType, message.MessageType);
        }

        [Fact]
        public void ShouldDelegateProcessingToMessageType()
        {
            var called = false;

            var mockType = new Mock<IMessageType>();
            mockType.Setup(x => x.ProcessMessage("John", "Hi")).Callback(() => called = true);

            var message = new Message("John", "Hi", mockType.Object);

            message.Process();

            Assert.True(called);
        }
    }
}
