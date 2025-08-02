using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;

namespace CoreLibrary.Tests.TestInfrastructure
{
    /// <summary> Test double that throws <see cref="ObjectDisposedException"/>
    /// after Dispose and records all sent messages. </summary>
    public sealed class FakeCommunicator : ICommunicator
    {
        private bool _disposed;
        public readonly List<Message> Sent = new();

        public Task StartAsync(CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public Task SendMessageAsync(Message m, CancellationToken token = default)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(FakeCommunicator));
            }
            Sent.Add(m);
            return Task.CompletedTask;
        }

#pragma warning disable CS0067
        public event EventHandler<Message>? MessageReceived;
#pragma warning disable

        public ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return ValueTask.CompletedTask;
            }
            // Note: this is a fake communicator, so we don't need to clean up resources.
            _disposed = true;
            return DisposeAsync(); // satisfy the async contract
        }
    }
}
