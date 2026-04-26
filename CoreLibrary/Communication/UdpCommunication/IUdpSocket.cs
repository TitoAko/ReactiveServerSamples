using System.Net.Sockets;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CoreLibrary.Tests")]
namespace CoreLibrary.Communication.UdpCommunication
{

    internal interface IUdpSocket : IAsyncDisposable
    {
        void Connect(string host, int port);
        ValueTask<int> SendAsync(ReadOnlyMemory<byte> datagram, CancellationToken token = default);
        ValueTask<UdpReceiveResult> ReceiveAsync(CancellationToken token = default);
    }
}
