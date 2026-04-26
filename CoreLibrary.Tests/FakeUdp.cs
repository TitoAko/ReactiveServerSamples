using System.Net;
using System.Net.Sockets;

using CoreLibrary.Communication.UdpCommunication;

internal sealed class FakeUdp : IUdpSocket
{
    private readonly Queue<byte[]> _inbox = new();

    public void Connect(string host, int port) { /* no-op for tests */ }

    public ValueTask<int> SendAsync(ReadOnlyMemory<byte> datagram, CancellationToken token = default)
    {
        _inbox.Enqueue(datagram.ToArray());
        return new ValueTask<int>(datagram.Length);
    }

    public async ValueTask<UdpReceiveResult> ReceiveAsync(CancellationToken token = default)
    {
        while (_inbox.Count == 0)
        {
            await Task.Delay(1, token);
        }

        var payload = _inbox.Dequeue();
        var ep = new IPEndPoint(IPAddress.Loopback, 0);
        return new UdpReceiveResult(payload, ep);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
