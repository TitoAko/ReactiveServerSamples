using CoreLibrary.Utilities;

namespace CoreLibrary.Tests.TestInfrastructure
{
    internal static class TestConfig
    {
        public static Configuration UdpLoopback(int? listenPort = null, int? targetPort = null)
        {
            int resolvedListenPort = listenPort ?? PortFinder.FreePort();
            return new()
            {
                Role = NodeRole.Client,
                Communicator = "UdpCommunicator",
                BindAddress = "127.0.0.1",
                TargetAddress = "127.0.0.1",
                Port = resolvedListenPort,
                ListenPort = resolvedListenPort,
                TargetPort = targetPort ?? resolvedListenPort,
                Username = "test",
                Password = "test"
            };
        }

        public static Configuration TcpLoopback(int? port = null)
        {
            return new()
            {
                Role = NodeRole.Client,
                Communicator = "TcpCommunicator",
                BindAddress = "127.0.0.1",
                TargetAddress = "127.0.0.1",
                Port = port ?? PortFinder.FreePort(),
                Username = "test",
                Password = "test"
            };
        }
    }
}