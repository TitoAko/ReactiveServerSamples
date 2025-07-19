using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Messaging.MessageTypes;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CoreLibrary.Communication.TcpCommunication
{
    // TODO: Split into TcpSender / TcpReceiver classes if TCP support is expanded or tested
    public class TcpCommunicator : ICommunicator
    {
        private readonly TcpListener _tcpListener;
        private readonly int _port;
        private readonly string _ipAddress;

        private CancellationTokenSource? _internalCts;
        private TcpClient? _connectedClient;

        public event Action<Message>? OnMessageReceived;

        public TcpCommunicator(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
            _tcpListener = new TcpListener(IPAddress.Parse(_ipAddress), _port);
        }

        public void StartListening(CancellationToken cancellationToken)
        {
            _tcpListener.Start();
            Console.WriteLine("TCP server is listening...");

            _internalCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _ = Task.Run(async () =>
            {
                while (!_internalCts.Token.IsCancellationRequested)
                {
                    try
                    {
                        _connectedClient = await _tcpListener.AcceptTcpClientAsync(_internalCts.Token).ConfigureAwait(false);
                        Console.WriteLine("TCP client connected.");

                        _ = HandleClientAsync(_connectedClient, _internalCts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine("TCP listening canceled.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"TCP listener error: {ex.Message}");
                    }
                }
            }, _internalCts.Token);
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            using var stream = client.GetStream();
            var buffer = new byte[1024];

            while (!token.IsCancellationRequested)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), token).ConfigureAwait(false);
                    if (bytesRead == 0)
                        break;

                    string messageContent = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    var message = new Message("Client", messageContent, new TextMessage());

                    OnMessageReceived?.Invoke(message);  // Raise event for testing/app logic
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    Console.WriteLine($"TCP client read error: {ex.Message}");
                    break;
                }
            }

            client.Close();
        }

        public async Task SendMessage(Message message, CancellationToken cancellationToken = default)
        {
            if (_connectedClient?.Connected != true)
            {
                Console.WriteLine("No TCP client connected.");
                return;
            }

            try
            {
                var stream = _connectedClient.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(message.Content);

                await stream.WriteAsync(data, 0, data.Length, cancellationToken).ConfigureAwait(false);
                Console.WriteLine($"[TCP SEND] {message.Sender}: {message.Content}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TCP send failed: {ex.Message}");
            }
        }

        public Message ReceiveMessage()
        {
            throw new NotSupportedException("Use OnMessageReceived event for TCP.");
        }

        public void Connect(CancellationToken cancellationToken)
        {
            Console.WriteLine("[TCP Client] Connect not implemented.");
        }

        public void Stop()
        {
            _internalCts?.Cancel();
            _tcpListener.Stop();
            _connectedClient?.Close();
        }

        public void Dispose()
        {
            Stop();
            _tcpListener.Server.Dispose();
        }
    }
}
