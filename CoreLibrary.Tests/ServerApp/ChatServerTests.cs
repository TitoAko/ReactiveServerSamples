using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;

using FluentAssertions;

using ServerApp;

namespace CoreLibrary.Tests.ServerApp;

public class ChatServerTests
{
    [Fact]
    public async Task Message_FromOneClient_IsBroadcastToOtherClients()
    {
        var server = new ChatServer();

        var sender = new FakeCommunicator();
        var receiver = new FakeCommunicator();

        server.AddClient(sender);
        server.AddClient(receiver);

        await sender.RaiseReceivedAsync(new Message("alice", "hello", MessageType.Chat));

        receiver.SentMessages.Should().ContainSingle(m => m.Content == "hello");
        sender.SentMessages.Should().BeEmpty();
    }

    [Fact]
    public async Task ExitMessage_DisposesSender_AndRemovesItFromBroadcasts()
    {
        var server = new ChatServer();

        var leaving = new FakeCommunicator();
        var remaining = new FakeCommunicator();

        server.AddClient(leaving);
        server.AddClient(remaining);

        await leaving.RaiseReceivedAsync(new Message("alice", "bye", MessageType.Exit));
        await remaining.RaiseReceivedAsync(new Message("bob", "after exit", MessageType.Chat));

        leaving.DisposeWasCalled.Should().BeTrue();
        leaving.SentMessages.Should().BeEmpty();
    }

    private sealed class FakeCommunicator : ICommunicator
    {
        public List<Message> SentMessages { get; } = new();
        public bool StartWasCalled { get; private set; }
        public bool DisposeWasCalled { get; private set; }

        public event EventHandler<Message>? MessageReceived;

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            StartWasCalled = true;
            return Task.CompletedTask;
        }

        public Task SendMessageAsync(Message message, CancellationToken cancellationToken = default)
        {
            SentMessages.Add(message);
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            DisposeWasCalled = true;
            return ValueTask.CompletedTask;
        }

        public async Task RaiseReceivedAsync(Message message)
        {
            MessageReceived?.Invoke(this, message);
            await Task.Delay(50);
        }
    }
}