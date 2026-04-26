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

    public string BindAddress { get; init; } = "0.0.0.0";   // where we listen
    public string TargetAddress { get; init; } = "server";    // where we connect
    public int Port { get; init; } = 9000;

    /// <summary>
    /// Local UDP/TCP port this node listens on.
    /// Defaults to Port for backward compatibility.
    /// </summary>
    public int ListenPort { get; init; } = 9000;

    /// <summary>
    /// Remote UDP/TCP port this node sends/connects to.
    /// Defaults to Port for backward compatibility.
    /// </summary>
    public int TargetPort { get; init; } = 9000;

    public string Endpoint => $"{TargetAddress}:{Port}";

    public const int DefaultUdpMaxPayload = 60_000;
    public int UdpMaxPayload { get; init; } = DefaultUdpMaxPayload;
}
