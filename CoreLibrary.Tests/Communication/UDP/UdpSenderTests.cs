using System.Text.Json;
using System.Text.Json.Serialization;

using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

using FluentAssertions;

namespace CoreLibrary.Tests.Communication.UDP
{
    public class UdpSenderTests
    {
        private static int MaxContentLen(Configuration cfg, string user = "cli")
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            int lo = 0, hi = cfg.UdpMaxPayload, best = 0;

            while (lo <= hi)
            {
                int mid = (lo + hi) / 2;
                var probe = new Message(user, new string('x', mid));
                int bytes = JsonSerializer.SerializeToUtf8Bytes(probe, UdpSender.JsonOpts).Length;

                if (bytes <= cfg.UdpMaxPayload)
                {
                    best = mid;
                    lo = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }

            return best;
        }

        [Fact]
        public async Task SendAsync_MaxContent_Fits()
        {
            var cfg = new Configuration { BindAddress = "127.0.0.1", TargetAddress = "127.0.0.1", ListenPort = 0, TargetPort = 9000, UdpMaxPayload = 60_000 };
            var fake = new FakeUdp();
            var sender = new UdpSender(fake, cfg);

            int max = MaxContentLen(cfg);
            var msg = new Message("cli", new string('x', max));

            await sender.SendAsync(msg);   // should not throw
        }

        [Fact]
        public async Task SendAsync_Over_Limit_Throws()
        {
            var cfg = new Configuration { BindAddress = "127.0.0.1", TargetAddress = "127.0.0.1", ListenPort = 0, TargetPort = 9000, UdpMaxPayload = 60_000 };
            var fake = new FakeUdp();
            var sender = new UdpSender(fake, cfg);

            var msg = new Message("cli", new string('x', 60_001));

            await sender.Invoking(s => s.SendAsync(msg))
                        .Should().ThrowAsync<ArgumentException>()
                        .WithMessage("*60 kB exceeded*");
        }
    }
}
