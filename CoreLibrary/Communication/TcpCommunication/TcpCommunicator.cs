using CoreLibrary.Messaging;
using CoreLibrary.Utilities;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace CoreLibrary.Communication.TcpCommunication
{
    /// <summary>
    /// Minimal TCP transport. Listens on the configured port and relays JSON
    /// chat packets.  Useful for smoke tests; production TODOs marked.
    /// </summary>
    public sealed class TcpCommunicator : ICommunicator
    {
        private readonly TcpSender _sender;
        private readonly TcpReceiver _receiver;

        public event EventHandler<Message>? MessageReceived
        {
            add => _receiver.MessageReceived += value;
            remove => _receiver.MessageReceived -= value;
        }

        public TcpCommunicator(Configuration cfg)
        {
            _sender = new TcpSender(cfg);
            _receiver = new TcpReceiver(cfg);
        }

        public Task SendMessageAsync(Message m, CancellationToken t = default)
            => _sender.SendAsync(m, t);

        public Task StartAsync(CancellationToken t = default)
            => _receiver.ListenAsync(t);

        public void Dispose()
        {
            _sender.Dispose();
            _receiver.Dispose();
        }
    }
}
