using System.Net.Sockets;

using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Tests.TestInfrastructure;

namespace CoreLibrary.Tests.EdgeCases
{
    public class UdpReceiverOversizeTests
    {
        [Fact]
        public async Task Oversize_Datagram_Is_Dropped()
        {
            var cfg = TestConfig.UdpLoopback() with { UdpMaxPayload = 60000 };
            await using var receiver = new UdpReceiver(cfg);

            var oversized = new byte[61000];
            using var udp = new UdpClient(cfg.TargetAddress, cfg.Port);
            await udp.SendAsync(oversized);

            var received = false;
            receiver.Received += (_, _) => received = true;

            var token = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            _ = receiver.ListenAsync(token.Token)
                .ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        Console.Error.WriteLine(t.Exception); // rethrow any exceptions
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);                     // fire & forget
            await Task.Delay(100);                          // small wait
            Assert.False(received);
        }

    }
}
