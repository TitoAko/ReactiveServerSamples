using CoreLibrary.Messaging;

namespace CoreLibrary.Tests.TestInfrastructure
{
    /// <summary>
    /// Lightweight test double – captures everything sent and allows you to push
    /// inbound messages manually via <see cref="Raise"/>.
    /// </summary>
    internal sealed class FakeCommunicator : ICommunicator
    {
        public readonly List<Message> Sent = new();

        public event EventHandler<Message>? MessageReceived;

        public Task StartAsync(CancellationToken t = default) => Task.CompletedTask;

        public Task SendMessageAsync(Message m, CancellationToken t = default)
        {
            Sent.Add(m);
            return Task.CompletedTask;
        }

        public void Raise(Message m) => MessageReceived?.Invoke(this, m);

        public void Dispose() { }
    }
}