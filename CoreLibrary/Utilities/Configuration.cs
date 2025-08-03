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

    public string Endpoint => $"{TargetAddress}:{Port}";

    public const int DefaultUdpMaxPayload = 60_000;
    public int UdpMaxPayload { get; init; } = DefaultUdpMaxPayload;
}
