using ClientApp;

using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;

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
            Console.SetIn(new StringReader("exit" + Environment.NewLine));

            await client.RunAsync(nonInteractive: false);
        }
        finally
        {
            Console.SetIn(originalIn);
        }

        fake.StartWasCalled.Should().BeTrue();
        fake.SentMessages.Should().ContainSingle(m => m.Type == MessageType.Exit);
    }
}