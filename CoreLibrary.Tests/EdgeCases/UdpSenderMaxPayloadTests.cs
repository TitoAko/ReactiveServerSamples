﻿using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;

using FluentAssertions;

namespace CoreLibrary.Tests.EdgeCases
{
    public class UdpSenderMaxPayloadTests
    {
        [Fact]
        public async Task Payload_Over_60KB_Throws()
        {
            var configuration = TestConfig.UdpLoopback(PortFinder.FreePort());

            var communicator = new UdpCommunicator(configuration);
            // 60 001 ASCII chars → 60 001 bytes
            var bigMessage = new Message("cli", new string('x', 60_001));

            _ = await communicator
                .Invoking(s => s.SendMessageAsync(bigMessage))
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("*60 kB exceeded*");
        }
    }
}