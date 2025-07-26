using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace CoreLibrary.Tests.Extra
{
    public class UdpSenderMaxPayloadTests
    {
        [Fact]
        public async Task SendMessageAsync_Rejects_WhenPayloadTooLarge()
        {
            // Arrange
            var configuration = new Configuration { IpAddress = "127.0.0.1", Port = 0 };
            var sender = new UdpSender(configuration, remotePort: 9999);

            // 61 kB content
            var bigContent = new string('x', 61_000);
            var message = new Message("test", bigContent);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => sender.SendMessageAsync(message));
        }
    }
}
