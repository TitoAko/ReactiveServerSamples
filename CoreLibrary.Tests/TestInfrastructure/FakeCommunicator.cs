// CoreLibrary.Tests/TestInfrastructure/FakeCommunicator.cs
using CoreLibrary.Messaging;

namespace CoreLibrary.Tests.TestInfrastructure
{
    /// <summary> Test double that throws <see cref="ObjectDisposedException"/>
    /// after Dispose and records all sent messages. </summary>
    public sealed class FakeCommunicator : ICommunicator, IDisposable
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
        public void Dispose() => _disposed = true;
    }
}
