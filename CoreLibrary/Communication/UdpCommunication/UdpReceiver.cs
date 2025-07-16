using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Messaging.MessageTypes;
using CoreLibrary.Utilities;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;

namespace CoreLibrary.Communication.UdpCommunication
{
    public class UdpReceiver
    {
        private readonly UdpClient _udpClient;
        private readonly Configuration _config;
        private readonly IMessageProcessor? _messageProcessor;
        private IDisposable? _subscription;
        private IDisposable? _stateSnapshot;

        public event Action<Message>? OnMessageReceived;

        public UdpReceiver(Configuration configuration, IMessageProcessor? messageProcessor = null)
        {
            _config = configuration;
            _udpClient = new UdpClient(_config.Port);
            _messageProcessor = messageProcessor;
        }
        public void StartObservables(CancellationToken cancellationToken)
        {
            Console.WriteLine("Rx observables starting...");

            // Create an observable of incoming packets
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

            // Subscribe with throttling and parsing
            _subscription = packetStream
                .Where(buf => buf.Length > 0)
                .Select(buf => Encoding.UTF8.GetString(buf))
                .Throttle(TimeSpan.FromMilliseconds(100))     // avoid flood
                .Subscribe(
                    async payload =>
                    {
                        Console.WriteLine($"Received: {payload}");
                        var message = new Message(_config.Username, payload, new TextMessage());
                        OnMessageReceived?.Invoke(message);

                        if (_messageProcessor != null)
                        {
                            await _messageProcessor.ProcessAsync(message); // optional processing
                        }
                    },
                    ex => Console.Error.WriteLine($"Stream error: {ex.Message}"),
                    () => Console.WriteLine("Stream completed.")
                );

            // A timer observable firing every second
            var ticker = Observable.Interval(TimeSpan.FromSeconds(1));

            _stateSnapshot = ticker
                .CombineLatest(packetStream.StartWith(Array.Empty<byte[]>()),
                    (tick, latestPacket) => new { Time = tick, Packet = latestPacket })
                .Subscribe(tick =>
                {

                    // Build and send state snapshot back to clients
                    Console.WriteLine($"Tick {tick.Time}, last packet size: {tick.Packet.Length}");
                });
        }

        public void DisposeResources()
        {
            Console.WriteLine("Shutting down.");
            _subscription?.Dispose();
            _stateSnapshot?.Dispose();
        }

        public void Stop()
        {
            Console.WriteLine("Stopping UdpReceiver");
            _subscription?.Dispose();
            _subscription = null;
            _stateSnapshot?.Dispose();
            _stateSnapshot = null;
            _udpClient.Close(); // Safe close
        }

        public void Dispose()
        {
            Stop();
            _udpClient.Dispose(); // Only dispose manually when fully shut down
        }

        public Message ReceiveMessage()
        {
            return new Message("Server", "Test message", new TextMessage());
        }
    }
}
