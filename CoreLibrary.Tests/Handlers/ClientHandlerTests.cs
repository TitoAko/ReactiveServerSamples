using CoreLibrary.Handlers;
using CoreLibrary.Messaging;
using MessageTypes = CoreLibrary.Messaging.MessageTypes;

namespace CoreLibrary.Tests.Handlers
{
    public class ClientHandlerTests
    {
        [Fact]
        public void ReceiveMessage_ShouldRaise_OnMessageReceived()
        {
            // Arrange
            var handler = new ClientHandler();
            var wasCalled = false;
            var message = new Message("TestUser", "Hello!", new MessageTypes.TextMessage());

            handler.OnMessageReceived += (msg) =>
            {
                wasCalled = true;
                Assert.Equal(message.Content, msg.Content);
                Assert.Equal(message.Sender, msg.Sender);
            };

            // Act
            handler.ReceiveMessage(message);

            // Assert
            Assert.True(wasCalled);
        }

        [Fact]
        public void Connect_ShouldRaise_OnConnect()
        {
            // Arrange
            var handler = new ClientHandler();
            var wasCalled = false;

            handler.OnConnect += () => wasCalled = true;

            // Act
            handler.Connect();

            // Assert
            Assert.True(wasCalled);
        }

        [Fact]
        public void Disconnect_ShouldRaise_OnDisconnect()
        {
            // Arrange
            var handler = new ClientHandler();
            var wasCalled = false;

            handler.OnDisconnect += () => wasCalled = true;

            // Act
            handler.Disconnect();

            // Assert
            Assert.True(wasCalled);
        }
    }
}
