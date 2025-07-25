using System.Net;
using System.Net.Sockets;

namespace CoreLibrary.Utilities
{
    /* -------------------- enums -------------------- */

    public enum NodeRole { Server, Client }          // used by new code
    public enum TransportKind { Udp, Tcp }          //    ”     ”   ”
    public enum AppType { Server, Client }          // legacy for AppLock

    /* -------------------- POCO --------------------- */

    public record Configuration
    {
        /* networking */
        public string BindAddress { get; init; } = "0.0.0.0";
        public string TargetAddress { get; init; } = "127.0.0.1";  // or service name

        public string IpAddress
        {
            get => TargetAddress;
            init { TargetAddress = value; }
        }
        public int Port { get; init; } = 9000;
        public TransportKind Transport { get; init; } = TransportKind.Udp;

        /* node identity */
        public NodeRole Role { get; init; } = NodeRole.Client;
        public string ClientId { get; init; } = $"cli-{Guid.NewGuid():N}";
        /* ---------- legacy aliases (AppLock, etc.) --------- */
        public AppType AppType => Role == NodeRole.Server ? AppType.Server : AppType.Client;
        public string AppName { get; init; } = "CorePunkChat";
        public string Username => ClientId;

        /* ----------------- helpers ------------------ */
        public bool IsEndpointBusy()
        {
            try
            {
                using var tcp = new TcpListener(IPAddress.Parse(IpAddress), Port);
                tcp.Start();
                return false;   // bind succeeded → port was free
            }
            catch (SocketException) { return true; }
        }
    }
}
