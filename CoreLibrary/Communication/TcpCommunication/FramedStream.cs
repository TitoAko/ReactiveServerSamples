namespace CoreLibrary.Communication.TcpCommunication
{

    /// <summary>Length-prefix (4-byte little-endian) framing over any stream.</summary>
    internal static class FramedStream
    {
        public static async Task WriteFrameAsync(
            Stream stream, ReadOnlyMemory<byte> payload, CancellationToken token = default)
        {
            byte[] len = BitConverter.GetBytes(payload.Length);
            await stream.WriteAsync(len, token).ConfigureAwait(false);
            await stream.WriteAsync(payload, token).ConfigureAwait(false);
        }

        public static async Task<byte[]> ReadFrameAsync(Stream stream, CancellationToken token = default)
        {
            var lenBuf = new byte[4];
            await stream.ReadAsync(lenBuf, token).ConfigureAwait(false);

            int len = BitConverter.ToInt32(lenBuf);
            if (len < 0 || len > 1_048_576)
            {
                throw new InvalidDataException($"Frame length {len} is invalid.");
            }

            var payload = new byte[len];
            await stream.ReadAsync(payload, token).ConfigureAwait(false);
            return payload;
        }
    }
}
