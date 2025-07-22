using CoreLibrary.Communication;
using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Utilities;

namespace CoreLibrary.Factories
{
    /// <summary>
    /// Creates the right <see cref="ICommunicator"/> for the given config.
    /// Reflection- and DI-free so it compiles instantly in small samples.
    /// </summary>
    public static class CommunicatorFactory
    {
        public static ICommunicator Create(Configuration cfg) =>
            cfg.Transport switch
            {
                TransportKind.Udp => new UdpCommunicator(cfg),
                TransportKind.Tcp => new TcpCommunicator(cfg),
                _ => throw new NotSupportedException(
                         $"Transport '{cfg.Transport}' not recognised.")
            };
    }
}
