using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace CoreLibrary.Communication.TcpCommunication;

public sealed class TcpSender : IDisposable
{
    private bool _disposed;
    private readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new MessageTypeConverter() }
    };
    private readonly TcpClient _tcp = new();
    private readonly Configuration _cfg;
    private bool _connected;

    public TcpSender(Configuration cfg) => _cfg = cfg;

    public async Task SendAsync(Message message, CancellationToken token = default)
    {
        if (!_connected)
        {
            _tcp.Connect(_cfg.TargetAddress, _cfg.Port);
            _connected = true;
        }

        if (_disposed)
            throw new ObjectDisposedException(nameof(TcpSender));
        string json = JsonSerializer.Serialize(message, _json);
        byte[] buffer = Encoding.UTF8.GetBytes(json);
        await _tcp.GetStream().WriteAsync(buffer, token).ConfigureAwait(false);
    }

    public void Dispose()
    {
        _disposed = true;
        _tcp.Dispose();
    }
}
