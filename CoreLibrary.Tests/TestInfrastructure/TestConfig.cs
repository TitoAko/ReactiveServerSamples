using CoreLibrary.Utilities;

namespace CoreLibrary.Tests.TestInfrastructure
{
    /// <summary>
    /// Helper factory methods that create ready-to-use Configuration
    /// instances for unit / integration tests so we avoid five-line object
    /// initialisers everywhere.
    /// </summary>
    internal static class TestConfig
    {
        /// <summary>
        /// TCP transport that binds on 0.0.0.0 and targets loopback.
        /// </summary>
        public static Configuration TcpLoopback(int port) => new()
        {
            BindAddress = "0.0.0.0",
            TargetAddress = "127.0.0.1",
            Port = port,
            Transport = TransportKind.Tcp
        };

        /// <summary>
        /// UDP transport variant – created by cloning the TCP version
        /// and overriding the Transport field.  Requires C# 10 record
        /// syntax; valid because Configuration is now a <c>record</c>.
        /// </summary>
        public static Configuration UdpLoopback(int port) =>
            TcpLoopback(port) with { Transport = TransportKind.Udp };
    }
}
