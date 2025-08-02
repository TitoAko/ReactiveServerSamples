using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;

namespace CoreLibrary.Tests.Extra
{
    public class UdpSenderMaxPayloadTests
    {
        [Fact]
        public async Task Throws_When_Exceeding_60KB()
        {
            var cfg = TestConfig.UdpLoopback();
            var sender = new UdpSender(cfg);

            // Build an oversize message (≈60 001 B once UTF-8 encoded)
            var largeText = new string('x', 60_001);
            var message = new Message(cfg.Username, largeText);

            await Assert.ThrowsAsync<ArgumentException>(() => sender.SendAsync(message));
        }
    }
}
