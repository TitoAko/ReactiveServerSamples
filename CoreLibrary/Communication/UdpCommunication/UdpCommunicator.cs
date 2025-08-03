using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace CoreLibrary.Communication.UdpCommunication
{
    public sealed class UdpCommunicator : ICommunicator, IAsyncDisposable
    {
        private readonly UdpSender _sender;
        private readonly UdpReceiver _receiver;
        private bool _disposed;

        public UdpCommunicator(Configuration cfg, int? remotePort = null)
        {
            _sender = new UdpSender(cfg, remotePort);
            _receiver = new UdpReceiver(cfg);
            _receiver.Received += (_, message) => MessageReceived?.Invoke(this, message);
        }

        public event EventHandler<Message>? MessageReceived;

        public Task SendMessageAsync(Message msg, CancellationToken t = default)
        {
            return _sender.SendAsync(msg, t);
        }

        public Task StartAsync(CancellationToken t = default)
        {
            return _receiver.ListenAsync(t)
                .ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        Console.Error.WriteLine(t.Exception); // rethrow any exceptions
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            await _sender.DisposeAsync();
            await _receiver.DisposeAsync();
        }
    }
}