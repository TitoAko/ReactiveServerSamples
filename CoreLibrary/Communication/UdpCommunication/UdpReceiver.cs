using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;

using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace CoreLibrary.Communication.UdpCommunication
{
    public sealed class UdpReceiver : IAsyncDisposable
    {
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        private readonly IUdpSocket _socket;
        private readonly Configuration _cfg;
        private CancellationTokenSource? _cts;
        private Task? _loop;
        private bool _disposed;

        public event EventHandler<Message>? Received;

        // Production ctor: bind to cfg.Port
        public UdpReceiver(Configuration cfg)
            : this(new UdpSocketAdapter(cfg.BindAddress, cfg.ListenPort), cfg) { }

        // Test-only ctor (seam)
        internal UdpReceiver(IUdpSocket socket, Configuration cfg)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _cfg = cfg ?? throw new ArgumentNullException(nameof(cfg));
        }

        public Task StartAsync(CancellationToken token = default)
        {
            if (_loop is not null)
            {
                return Task.CompletedTask;
            }
            _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            _loop = ListenAsync(_cts.Token);
            return Task.CompletedTask;
        }

        public async Task ListenAsync(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                UdpReceiveResult result;
                try
                {
                    result = await _socket.ReceiveAsync(token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                System.Diagnostics.Debug.WriteLine($"UDP recv {result.Buffer.Length} bytes");
                if (result.Buffer.Length > _cfg.UdpMaxPayload)
                {
                    continue;
                }

                var msg = JsonSerializer.Deserialize<Message>(result.Buffer, JsonOpts);
                if (msg is not null)
                {
                    Received?.Invoke(this, msg);
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;

            try { _cts?.Cancel(); } catch { /* ignore */ }

            if (_loop is not null)
            {
                try { await _loop.ConfigureAwait(false); } catch { /* ignore */ }
            }

            await _socket.DisposeAsync().ConfigureAwait(false);
            _cts?.Dispose();
        }
    }
}
