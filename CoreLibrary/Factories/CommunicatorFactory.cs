using CoreLibrary.Communication.TcpCommunication;
using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Interfaces;
using CoreLibrary.Utilities;

namespace CoreLibrary.Factories
{
    /// <summary>
    /// Creates the right <see cref="ICommunicator"/> for the given config.
    /// Reflection- and DI-free so it compiles instantly in small samples.
    /// </summary>
    public static class CommunicatorFactory
    {
        public static ICommunicator Create(Configuration configuration)
        {
            return configuration.Transport switch
            {
                TransportKind.Udp => new UdpCommunicator(configuration),
                TransportKind.Tcp => new TcpCommunicator(configuration),
                _ => throw new NotSupportedException(
                         $"Transport '{configuration.Transport}' not recognised.")
            };
        }
    }
}
