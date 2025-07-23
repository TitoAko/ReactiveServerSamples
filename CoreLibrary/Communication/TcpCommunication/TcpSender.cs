using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace CoreLibrary.Communication.TcpCommunication;

public sealed class TcpSender : IDisposable
{
    private readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new MessageTypeConverter() }
    };
    private readonly TcpClient _tcp = new();

    public TcpSender(Configuration cfg)
    {
        _tcp.Connect(cfg.TargetAddress, cfg.Port);
    }

    public async Task SendAsync(Message message, CancellationToken token = default)
    {
        string json = JsonSerializer.Serialize(message, _json);
        byte[] buffer = Encoding.UTF8.GetBytes(json);
        await _tcp.GetStream().WriteAsync(buffer, token).ConfigureAwait(false);
    }

    public void Dispose() => _tcp.Dispose();
}
