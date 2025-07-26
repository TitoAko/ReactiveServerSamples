using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace CoreLibrary.Communication.UdpCommunication
{
    /// <summary>
    /// “Façade” that glues <see cref="UdpSender"/> and <see cref="UdpReceiver"/>
    /// to satisfy <see cref="ICommunicator"/>.
    /// </summary>
    public sealed class UdpCommunicator : ICommunicator
    {
        private readonly UdpSender _sender;
        private readonly UdpReceiver _receiver;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public event EventHandler<Message>? MessageReceived;

        public UdpCommunicator(Configuration configuration, int? remotePort = null)
        {
            _sender = new UdpSender(configuration, remotePort ?? configuration.Port);
            _receiver = new UdpReceiver(configuration);
            _receiver.MessageReceived += (_, message) => MessageReceived?.Invoke(this, message);
        }

        public Task SendMessageAsync(Message message,
                                     CancellationToken token = default)
        {
            return _sender.SendMessageAsync(message, token);
        }

        public async Task StartAsync(CancellationToken token = default)
        {
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, _cancellationTokenSource.Token);
            await _receiver.ListenAsync(linked.Token).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _receiver.Dispose();
            _sender.Dispose();
        }
    }
}
