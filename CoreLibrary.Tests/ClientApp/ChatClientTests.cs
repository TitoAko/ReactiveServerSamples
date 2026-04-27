using ClientApp;

using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;

using FluentAssertions;

namespace CoreLibrary.Tests.ClientApp;

public class ChatClientTests
{
    [Fact]
    public async Task RunAsync_NonInteractive_StartsCommunicator()
    {
        var fake = new FakeCommunicator();
        using var client = new ChatClient(fake, "test");

        await client.RunAsync(nonInteractive: true);

        fake.StartWasCalled.Should().BeTrue();
        fake.SentMessages.Should().BeEmpty();
    }

    [Fact]
    public async Task RunAsync_WhenUserTypesExit_SendsExitMessage()
    {
        var fake = new FakeCommunicator();
        using var client = new ChatClient(fake, "test");

        var originalIn = Console.In;
        try
        {
            Console.SetIn(new StringReader("exit"));

            await client.RunAsync(nonInteractive: false);
        }
        finally
        {
            Console.SetIn(originalIn);
        }

        fake.StartWasCalled.Should().BeTrue();
        fake.SentMessages.Should().ContainSingle(m => m.Type == MessageType.Exit);
    }

    private sealed class FakeCommunicator : ICommunicator
    {
        public bool StartWasCalled { get; private set; }
        public bool DisposeWasCalled { get; private set; }
        public List<Message> SentMessages { get; } = new();

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
    }
}