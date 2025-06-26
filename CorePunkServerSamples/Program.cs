using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;

namespace ServerApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MyObservables();
        }

        static void MyObservables()
        {
            var udpClient = new UdpClient(9000);
            var endpoint = new IPEndPoint(IPAddress.Any, 0);

            // Create an observable of incoming packets
            var packetStream = Observable.Create<byte[]>(async (obs, ct) =>
            {
                while (!ct.IsCancellationRequested)
                {
                    var result = await udpClient.ReceiveAsync(ct);
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
                    payload => Console.WriteLine($"Received: {payload}"),
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
                subscription.Dispose();
                stateSnapshot.Dispose();
                udpClient.Close();
                Console.WriteLine("Shutting down.");
            };
        }
    }
}
