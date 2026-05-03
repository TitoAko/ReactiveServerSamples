using CoreLibrary.Communication.TcpCommunication;

using FluentAssertions;

namespace CoreLibrary.Tests.Communication.TCP
{
    public class FramedStreamTests
    {
        [Fact]
        public async Task WriteFrameAsync_WritesLengthPrefixAndPayload()
        {
            using var stream = new MemoryStream();
            byte[] payload = [1, 2, 3, 4, 5];

            await FramedStream.WriteFrameAsync(stream, payload);

            byte[] bytes = stream.ToArray();

            bytes.Length.Should().Be(4 + payload.Length);

            int lengthPrefix = BitConverter.ToInt32(bytes.AsSpan(0, 4));
            lengthPrefix.Should().Be(payload.Length);

            bytes.Skip(4).Should().Equal(payload);
        }

        [Fact]
        public async Task ReadFrameAsync_ReadsCompleteFrame()
        {
            byte[] payload = [10, 20, 30];
            using var stream = new MemoryStream();

            await FramedStream.WriteFrameAsync(stream, payload);
            stream.Position = 0;

            byte[] result = await FramedStream.ReadFrameAsync(stream);

            result.Should().Equal(payload);
        }

        [Fact]
        public async Task ReadFrameAsync_Throws_OnNegativeLength()
        {
            byte[] invalidLength = BitConverter.GetBytes(-1);
            using var stream = new MemoryStream(invalidLength);

            await Assert.ThrowsAsync<InvalidDataException>(() =>
                FramedStream.ReadFrameAsync(stream));
        }

        [Fact]
        public async Task ReadFrameAsync_Throws_OnTooLargeLength()
        {
            byte[] invalidLength = BitConverter.GetBytes(1_048_577);
            using var stream = new MemoryStream(invalidLength);

            await Assert.ThrowsAsync<InvalidDataException>(() =>
                FramedStream.ReadFrameAsync(stream));
        }

        [Fact]
        public async Task ReadFrameAsync_Throws_OnTruncatedPayload()
        {
            using var stream = new MemoryStream();

            byte[] declaredLength = BitConverter.GetBytes(5);
            stream.Write(declaredLength);
            stream.Write([1, 2]); // only 2 bytes instead of 5
            stream.Position = 0;

            await Assert.ThrowsAsync<EndOfStreamException>(() =>
                FramedStream.ReadFrameAsync(stream));
        }
    }
}