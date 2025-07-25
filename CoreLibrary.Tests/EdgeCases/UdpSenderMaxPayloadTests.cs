using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;
using CoreLibrary.Utilities;
using FluentAssertions;

namespace CoreLibrary.Tests.EdgeCases;

public class UdpSenderMaxPayloadTests
{
    [Fact]
    public async Task Payload_Over_60KB_Throws()
    {
        var cfg = TestConfig.UdpLoopback(PortFinder.FreePort());

        var comm = new UdpCommunicator(cfg);
        // 60 001 ASCII chars → 60 001 bytes
        var big = new Message("cli", new string('x', 60_001));

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            comm.SendMessageAsync(big).TimeoutAfter(TimeSpan.FromSeconds(1)));

        ex.Message.Should().Contain("exceeds 60 kB");
    }
}
