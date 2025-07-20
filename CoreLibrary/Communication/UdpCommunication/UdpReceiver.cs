using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Messaging.MessageTypes;
using CoreLibrary.Utilities;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;

namespace CoreLibrary.Communication.UdpCommunication
{
    /// <summary>
    /// Handles receiving UDP packets, exposing an observable stream of messages.
    /// </summary>
    public class UdpReceiver
    {
        private readonly UdpClient _udpClient;
        private readonly Configuration _config;
        private readonly IMessageProcessor? _messageProcessor;
        private IDisposable? _subscription;
        private IDisposable? _stateSnapshot;

        /// <summary>
        /// Fired when a valid message is received and parsed.
        /// </summary>
        public event Action<Message>? OnMessageReceived;

        /// <summary>
        /// Initializes the UDP receiver and prepares it for asynchronous listening.
        /// </summary>
        public UdpReceiver(Configuration configuration, IMessageProcessor? messageProcessor = null)
        {
            _config = configuration;
            _udpClient = new UdpClient(_config.Port);
            _messageProcessor = messageProcessor;
        }

        /// <summary>
        /// Starts the reactive observables to receive and deserialize messages.
        /// </summary>
        public void StartObservables(CancellationToken cancellationToken)
        {
            Console.WriteLine("Rx observables starting...");

            var packetStream = Observable.Create<byte[]>(async (observer, ct) =>
            {
                while (!ct.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        UdpReceiveResult result = await _udpClient.ReceiveAsync(ct);
                        observer.OnNext(result.Buffer);
                    }
                    catch (OperationCanceledException)
                    {
                        observer.OnCompleted();
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Receive failed: {ex.Message}");
                        observer.OnError(ex);
                        break;
                    }
                }
            });

            _subscription = packetStream
                .Where(buf => buf.Length > 0)
                .Select(buf => Encoding.UTF8.GetString(buf))
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Subscribe(
                    async payload =>
                    {
                        try
                        {
                            var message = JsonSerializer.Deserialize<Message>(payload, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                                AllowTrailingCommas = true,
                                Converters = { new MessageTypeConverter() }
                            });

                            if (message is not null)
                            {
                                OnMessageReceived?.Invoke(message);
                                if (_messageProcessor != null)
                                    await _messageProcessor.ProcessAsync(message);
                            }
                        }
                        catch (JsonException ex)
                        {
                            Console.Error.WriteLine($"Failed to deserialize message: {ex.Message}");
                        }
                    },
                    ex => Console.Error.WriteLine($"Stream error: {ex.Message}"),
                    () => Console.WriteLine("Stream completed.")
                );

            var ticker = Observable.Interval(TimeSpan.FromSeconds(1));

            _stateSnapshot = ticker
                .CombineLatest(packetStream.StartWith(Array.Empty<byte[]>()),
                    (tick, latestPacket) => new { Time = tick, Packet = latestPacket })
                .Subscribe(tick =>
                {
                    Console.WriteLine($"Tick {tick.Time}, last packet size: {tick.Packet.Length}");
                });
        }

        /// <summary>
        /// Stops the receiver and disposes observables.
        /// </summary>
        public void Stop()
        {
            Console.WriteLine("Stopping UdpReceiver");
            _subscription?.Dispose();
            _subscription = null;
            _stateSnapshot?.Dispose();
            _stateSnapshot = null;
            _udpClient.Close();
        }

        /// <summary>
        /// Releases all internal resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
            _udpClient.Dispose();
        }

        /// <summary>
        /// Returns a test message. Only used for diagnostics or placeholder behavior.
        /// </summary>
        public Message ReceiveMessage()
        {
            return new Message("Server", "Test message", new TextMessage());
        }

        /// <summary>
        /// Disposes internal observables.
        /// </summary>
        public void DisposeResources()
        {
            Console.WriteLine("Shutting down.");
            _subscription?.Dispose();
            _stateSnapshot?.Dispose();
        }
    }
}
