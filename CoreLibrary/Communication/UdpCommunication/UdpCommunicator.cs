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
        private readonly CancellationTokenSource _cts = new();

        public event EventHandler<Message>? MessageReceived;

        public UdpCommunicator(Configuration cfg, int? remotePort = null)
        {
            _sender = new UdpSender(cfg, remotePort ?? cfg.Port);
            _receiver = new UdpReceiver(cfg);
            _receiver.MessageReceived += (_, m) => MessageReceived?.Invoke(this, m);
        }

        public Task SendMessageAsync(Message message,
                                     CancellationToken token = default) =>
            _sender.SendMessageAsync(message, token);

        public async Task StartAsync(CancellationToken token = default)
        {
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, _cts.Token);
            await _receiver.ListenAsync(linked.Token).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _receiver.Dispose();
            _sender.Dispose();
        }
    }
}
