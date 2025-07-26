using CoreLibrary.Messaging;

namespace ServerApp.Models
{
    /// <summary>
    /// Wraps a single remote user: owns its communicator and exposes a typed event.
    /// </summary>
    public sealed class ClientConnection : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly ICommunicator _communicator;

        public string Id { get; }
        public event EventHandler<Message>? Received;

        public ClientConnection(ICommunicator communicator, string? id = null)
        {
            _communicator = communicator ?? throw new ArgumentNullException(nameof(communicator));
            Id = id ?? Guid.NewGuid().ToString("N");

            _communicator.MessageReceived += (_, message) => Received?.Invoke(this, message);
        }

        public Task StartAsync()
        {
            return _communicator.StartAsync(_cancellationTokenSource.Token);
        }

        public Task SendAsync(Message message)
        {
            return _communicator.SendMessageAsync(message, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _communicator.Dispose();
        }
    }
}