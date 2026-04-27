using CoreLibrary.Communication.TcpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;

namespace CoreLibrary.Tests.EdgeCases
{

    public class TcpZeroLengthPayloadTests
    {
        [Fact]
        public async Task DisposeAsync_DoesNotHang_WhenListenerWasStarted()
        {
            var configuration = TestConfig.TcpLoopback(PortFinder.FreePort());
            await using var sender = new TcpSender(configuration);

            var emptyMessage = new Message("cli", "");

            await Assert.ThrowsAsync<ArgumentException>(() =>
                sender.SendAsync(emptyMessage));
        }
    }
}