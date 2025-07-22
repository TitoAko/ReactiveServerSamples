namespace CoreLibrary.Tests.TestInfrastructure;

/// <summary>Reserves a free TCP/UDP port that OS chooses at runtime.</summary>
internal static class PortFinder
{
    public static int FreePort()
    {
        var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        int port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
