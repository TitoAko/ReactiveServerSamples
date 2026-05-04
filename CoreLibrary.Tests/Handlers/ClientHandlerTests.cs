using CoreLibrary.Handlers;
using CoreLibrary.Messaging;

using FluentAssertions;

namespace CoreLibrary.Tests.Handlers
{
    public class ClientHandlerTests
    {
        [Fact]
        public void ReceiveMessage_Raises_OnMessageReceived_WithSameMessage()
        {
            var handler = new ClientHandler();
            var expected = new Message("alice", "hello", MessageType.Chat);

            Message? received = null;
            handler.OnMessageReceived += message => received = message;

            handler.ReceiveMessage(expected);

            received.Should().BeSameAs(expected);
        }

        [Fact]
        public void Connect_Raises_OnConnect()
        {
            var handler = new ClientHandler();

            var wasCalled = false;
            handler.OnConnect += () => wasCalled = true;

            handler.Connect();

            wasCalled.Should().BeTrue();
        }

        [Fact]
        public void Disconnect_Raises_OnDisconnect()
        {
            var handler = new ClientHandler();

            var wasCalled = false;
            handler.OnDisconnect += () => wasCalled = true;

            handler.Disconnect();

            wasCalled.Should().BeTrue();
        }

        [Fact]
        public void Methods_DoNotThrow_WhenNoSubscribers()
        {
            var handler = new ClientHandler();

            Action receive = () => handler.ReceiveMessage(new Message("bob", "ping"));
            Action connect = handler.Connect;
            Action disconnect = handler.Disconnect;

            receive.Should().NotThrow();
            connect.Should().NotThrow();
            disconnect.Should().NotThrow();
        }
    }
}