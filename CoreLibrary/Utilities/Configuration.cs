namespace CoreLibrary.Utilities;

public enum NodeRole { Server, Client, Monitor }

/// <summary>
/// Immutable runtime settings (JSON → ENV → CLI).
/// </summary>
public record Configuration
{
    public NodeRole Role { get; init; } = NodeRole.Client;
    public string Username { get; init; } = "guest";
    public string Password { get; init; } = "guest";

    /// <example>"UdpCommunicator" or "TcpCommunicator"</example>
    public string Communicator { get; init; } = "UdpCommunicator";

    /// <summary>Local address this node binds/listens on.</summary>
    public string BindAddress { get; init; } = "0.0.0.0";

    /// <summary>Local UDP/TCP port this node listens on.</summary>
    public int ListenPort { get; init; } = 9000;

    /// <summary>Remote address this node sends/connects to.</summary>
    public string TargetAddress { get; init; } = "server";

    /// <summary>Remote UDP/TCP port this node sends/connects to.</summary>
    public int TargetPort { get; init; } = 9000;

    public string Endpoint => $"{TargetAddress}:{TargetPort}";

    public const int DefaultUdpMaxPayload = 60_000;
    public int UdpMaxPayload { get; init; } = DefaultUdpMaxPayload;
}