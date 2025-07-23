using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace CoreLibrary.Communication.TcpCommunication;

public sealed class TcpReceiver
{
    private readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new MessageTypeConverter() }
    };
    private readonly TcpListener _listener;
    public event EventHandler<Message>? MessageReceived;

    public TcpReceiver(Configuration cfg)
    {
        _listener = new TcpListener(IPAddress.Parse(cfg.BindAddress), cfg.Port);
    }

    public async Task ListenAsync(CancellationToken token = default)
    {
        _listener.Start();
        while (!token.IsCancellationRequested)
        {
            var client = await _listener.AcceptTcpClientAsync(token);
            _ = HandleClientAsync(client, token);          // fire-and-forget
        }
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken token)
    {
        var stream = client.GetStream();
        var buffer = new byte[4096];

        while (!token.IsCancellationRequested)
        {
            int read = await stream.ReadAsync(buffer, token);
            if (read == 0) break;                          // remote closed

            string json = Encoding.UTF8.GetString(buffer, 0, read);
            var msg = JsonSerializer.Deserialize<Message>(json, _json)!;
            MessageReceived?.Invoke(this, msg);
        }
        client.Dispose();
    }

    public void Dispose() => _listener.Stop();
}
