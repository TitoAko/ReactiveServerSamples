using System.Text.Json;

using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

using FluentAssertions;

namespace CoreLibrary.Tests.Communication.UDP
{
    public class UdpReceiverTests
    {
        [Fact(Timeout = 2000)]
        public async Task Receiver_Raises_On_ValidPayload()
        {
            var cfg = new Configuration { BindAddress = "127.0.0.1", Port = 9000, UdpMaxPayload = 60_000 };
            var fake = new FakeUdp();
            var recv = new UdpReceiver(fake, cfg);

            var seen = new List<Message>();
            recv.Received += (_, m) => seen.Add(m);

            _ = recv.StartAsync(); // start loop

            var msgBytes = JsonSerializer.SerializeToUtf8Bytes(new Message("cli", "ping"));
            await fake.SendAsync(msgBytes); // feed one datagram

            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (seen.Count == 0 && sw.ElapsedMilliseconds < 1000)
            {
                await Task.Delay(10);
            }

            seen.Should().ContainSingle();
            seen[0].Content.Should().Be("ping");

            await recv.DisposeAsync();
        }

        [Fact(Timeout = 2000)]
        public async Task Receiver_Drops_Oversize()
        {
            var cfg = new Configuration { BindAddress = "127.0.0.1", Port = 9001, UdpMaxPayload = 60_000 };
            var fake = new FakeUdp();
            var recv = new UdpReceiver(fake, cfg);

            var fired = false;
            recv.Received += (_, __) => fired = true;

            _ = recv.StartAsync();

            var tooBig = new byte[60_001];
            await fake.SendAsync(tooBig);

            await Task.Delay(150); // enough for one receive iteration
            fired.Should().BeFalse();

            await recv.DisposeAsync();
        }

        [Fact(Timeout = 2000)]
        public async Task Receiver_Stops_On_Cancellation()
        {
            var cfg = new Configuration { BindAddress = "127.0.0.1", Port = 9002, UdpMaxPayload = 60_000 };
            var fake = new FakeUdp();
            var recv = new UdpReceiver(fake, cfg);

            using var cts = new CancellationTokenSource(100);
            var loop = recv.ListenAsync(cts.Token);

            await loop; // completes when token cancels
        }
    }
}
