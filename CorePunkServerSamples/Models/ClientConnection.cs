using CoreLibrary.Communication;
using CoreLibrary.Messaging;

namespace ServerApp.Models
{
    /// <summary>
    /// Wraps a single remote user: owns its communicator and exposes a typed event.
    /// </summary>
    public sealed class ClientConnection : IDisposable
    {
        private readonly CancellationTokenSource _cts = new();
        private readonly ICommunicator _comm;

        public string Id { get; }
        public event EventHandler<Message>? Received;

        public ClientConnection(ICommunicator communicator, string? id = null)
        {
            _comm = communicator ?? throw new ArgumentNullException(nameof(communicator));
            Id = id ?? Guid.NewGuid().ToString("N");

            _comm.MessageReceived += (_, m) => Received?.Invoke(this, m);
        }

        public Task StartAsync() => _comm.StartAsync(_cts.Token);

        public Task SendAsync(Message m) => _comm.SendMessageAsync(m, _cts.Token);

        public void Dispose()
        {
            _cts.Cancel();
            _comm.Dispose();
        }
    }
}