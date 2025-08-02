using CoreLibrary.Communication.TcpCommunication;
using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Interfaces;
using CoreLibrary.Utilities;

namespace CoreLibrary.Factories;

public static class CommunicatorFactory
{
    public static ICommunicator Create(Configuration cfg)
    {
        return cfg.Communicator.ToLowerInvariant() switch
        {
            "udpcommunicator" => new UdpCommunicator(cfg),
            "tcpcommunicator" => new TcpCommunicator(cfg),
            _ => throw new ArgumentException(
                     $"Unsupported communicator: {cfg.Communicator}", nameof(cfg))
        };
    }
}
