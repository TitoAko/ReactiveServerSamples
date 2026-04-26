using System.Net;
using System.Net.Sockets;

namespace CoreLibrary.Communication.UdpCommunication
{
    internal sealed class UdpSocketAdapter : IUdpSocket
    {
        private readonly UdpClient _client;

        public UdpSocketAdapter(string bindAddress, int localPort = 0)
        {
            IPAddress ip = bindAddress switch
            {
                null or "" or "0.0.0.0" => IPAddress.Any,
                "::" or "::0" => IPAddress.IPv6Any,
                _ => IPAddress.Parse(bindAddress)
            };
            _client = new UdpClient(new IPEndPoint(ip, localPort));   // proper bind
        }

        public void Connect(string host, int port)
        {
            // If caller passes an unspecified “remote”, normalize to loopback
            if (string.IsNullOrWhiteSpace(host) || host == "0.0.0.0" || host == "::" || host == "::0")
            {
                host = "127.0.0.1";
            }

            _client.Connect(host, port);
        }

        public ValueTask<int> SendAsync(ReadOnlyMemory<byte> datagram, CancellationToken token = default)
        {
            var arr = datagram.ToArray();
            int sent = _client.Send(arr, arr.Length); // robust sync send for tests
            return new ValueTask<int>(sent);
        }

        public ValueTask<UdpReceiveResult> ReceiveAsync(CancellationToken token = default)
        {
            return _client.ReceiveAsync(token);
        }

        public ValueTask DisposeAsync()
        {
            _client.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
