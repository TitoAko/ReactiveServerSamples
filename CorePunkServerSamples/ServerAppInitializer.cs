using CoreLibrary.Communication;
using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Messaging.MessageTypes;
using CoreLibrary.Utilities;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;

namespace ServerApp
{
    public class ServerAppInitializer
    {
        private readonly Configuration _config;
        private readonly AppLock _appLock;
        private readonly ICommunicator _communicator;
        private readonly UdpClient _udpClient;

        public ServerAppInitializer()
        {
            _config = new Configuration("launchSettings.json");  // Use Configuration to hold all the parameters
            _appLock = new AppLock();
            _udpClient = new UdpClient(_config.Port);
            _communicator = new UdpCommunicator(_udpClient, _config.IpAddress, _config.Port, _config.Username);
        }

        public void InitializeServer()
        {
            // Using ClientLock to check if the server is already running
            if (_appLock.IsInstanceRunning(_config))
            {
                Console.WriteLine("The server is already running on this IP/Port.");
            }

            // Initialize the server logic here...
            // Start the server
            Console.WriteLine("Server is starting...");

            StartObservables();
        }

        public void ReleaseLock()
        {
            _appLock.ReleaseLock();  // Release the lock after the server finishes
        }

        public void StartObservables()
        {
            Console.WriteLine($"UDP server is listening on port {_config.Port}...");

            // initialize ChatServer
            var chatServer = new ChatServer(_communicator, new UserManager());

            // Create an observable of incoming packets
            var packetStream = Observable.Create<byte[]>(async (obs, ct) =>
            {
                while (!ct.IsCancellationRequested)
                {
                    var result = await _udpClient.ReceiveAsync(ct);
                    obs.OnNext(result.Buffer);
                }
                obs.OnCompleted();
            });

            // Subscribe with throttling and parsing
            var subscription = packetStream
                .Where(buf => buf.Length > 0)
                .Select(buf => Encoding.UTF8.GetString(buf))
                .Throttle(TimeSpan.FromMilliseconds(100))     // avoid flood
                .Subscribe(
                    payload =>
                    {
                        Console.WriteLine($"Received: {payload}");
                        var message = new Message(_config.Username, payload, new TextMessage());
                        chatServer.ProcessMessage(message);  // Process the message
                    },
                    ex => Console.Error.WriteLine($"Error: {ex.Message}"),
                    () => Console.WriteLine("Stream completed.")
                );

            // A timer observable firing every second
            var ticker = Observable.Interval(TimeSpan.FromSeconds(1));

            var stateSnapshot = ticker
                .CombineLatest(packetStream.StartWith(Array.Empty<byte[]>()),
                    (tick, latestPacket) => new { Time = tick, Packet = latestPacket })
                .Subscribe(snapshot =>
                {
                    // Build and send state snapshot back to clients
                    Console.WriteLine($"Tick {snapshot.Time}, last packet size: {snapshot.Packet.Length}");
                });

            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Shutting down.");
                subscription.Dispose();
                stateSnapshot.Dispose();
                _udpClient.Close();
                ReleaseLock();
            };
        }
    }
}
