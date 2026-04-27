using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;

namespace CoreLibrary.Tests.TestInfrastructure
{
    /// <summary>
    /// Test double that records sent messages and throws after disposal.
    /// </summary>
    public sealed class FakeCommunicator : ICommunicator
    {
        private bool _disposed;

        public bool DisposeWasCalled { get; private set; }
        public bool StartWasCalled { get; private set; }

        public List<Message> SentMessages { get; } = new();

        public event EventHandler<Message>? MessageReceived;

        public Task StartAsync(CancellationToken token = default)
        {
            StartWasCalled = true;
            return Task.CompletedTask;
        }

        public Task SendMessageAsync(Message m, CancellationToken token = default)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(FakeCommunicator));
            }

            SentMessages.Add(m);
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return ValueTask.CompletedTask;
            }

            _disposed = true;
            DisposeWasCalled = true;

            return ValueTask.CompletedTask;
        }
    }
}