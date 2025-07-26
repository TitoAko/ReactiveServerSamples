using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace CoreLibrary.Communication.UdpCommunication
{
    /// <summary>
    /// Listens for UDP datagrams and raises <see cref="MessageReceived"/>.
    /// </summary>
    public class UdpReceiver : IDisposable
    {
        private readonly UdpClient _udpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Fired when a valid message is received and parsed.
        /// </summary>
        public event EventHandler<Message>? MessageReceived;

        /// <summary>
        /// Initializes the UDP receiver and prepares it for asynchronous listening.
        /// </summary>
        public UdpReceiver(Configuration configuration)
        {
            var local = new IPEndPoint(
                IPAddress.Any,
                configuration.Port);

            _udpClient = new UdpClient(local);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new MessageTypeConverter() }
            };
        }

        /// <summary>
        /// Starts the infinite read loop. Call once, ideally from a background Task.
        /// </summary>
        public async Task ListenAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    UdpReceiveResult result =
                        await _udpClient.ReceiveAsync(cancellationToken).ConfigureAwait(false);

                    string json = Encoding.UTF8.GetString(result.Buffer);
                    var message = JsonSerializer.Deserialize<Message>(json, _jsonOptions);

                    if (message is not null)
                    {
                        MessageReceived?.Invoke(this, message);
                    }
                }
                catch (OperationCanceledException)
                {
                    // normal shutdown, swallow silently
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"UDP receive error: {ex}");
                    // optional: raise an error event so the app can decide what to do
                }
            }
        }

        /// <summary>
        /// Releases all internal resources.
        /// </summary>
        public void Dispose()
        {
            _udpClient.Dispose();
        }
    }
}
