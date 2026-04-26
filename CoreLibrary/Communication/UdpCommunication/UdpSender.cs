using System.Text.Json;
using System.Text.Json.Serialization;

using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace CoreLibrary.Communication.UdpCommunication
{
    public sealed class UdpSender : IAsyncDisposable
    {
        internal static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        private readonly IUdpSocket _socket;
        private readonly Configuration _cfg;
        private bool _disposed;
        private bool _connected;

        public UdpSender(Configuration cfg)
    : this(new UdpSocketAdapter(cfg.BindAddress, 0), cfg) { }

        internal UdpSender(IUdpSocket socket, Configuration cfg)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _cfg = cfg ?? throw new ArgumentNullException(nameof(cfg));
        }

        // Test-only ctor (seam)
        internal UdpSender(IUdpSocket socket, Configuration cfg, int? remotePort = null)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _cfg = cfg ?? throw new ArgumentNullException(nameof(cfg));
        }

        public async Task SendAsync(Message msg, CancellationToken token = default)
        {
            // Lazy connect on first send to avoid early exceptions with 0.0.0.0
            if (!_connected)
            {
                var host = NormalizeRemote(_cfg.TargetAddress);
                var port = _cfg.TargetPort;
                _socket.Connect(host, port);
                _connected = true;
            }

            var bytes = JsonSerializer.SerializeToUtf8Bytes(msg, JsonOpts);
            if (bytes.Length > _cfg.UdpMaxPayload)
            {
                throw new ArgumentException("UDP payload limit 60 kB exceeded.", nameof(msg));
            }

            await _socket.SendAsync(bytes, token);
        }

        private static string NormalizeRemote(string host)
        {
            // Treat unspecified addresses as loopback for local dev/test
            if (string.IsNullOrWhiteSpace(host) || host == "0.0.0.0" || host == "::" || host == "::0")
            {
                return "127.0.0.1";
            }

            return host;
        }

        public ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return ValueTask.CompletedTask;
            }

            _disposed = true;
            return _socket.DisposeAsync();
        }
    }
}
