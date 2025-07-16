using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Messaging.MessageTypes;
using CoreLibrary.Utilities;

namespace CoreLibrary.Communication.UdpCommunication
{
    public class UdpCommunicator : ICommunicator
    {
        private readonly UdpReceiver _udpReceiver;
        private readonly UdpSender _udpSender;
        private readonly string _userName;

        public event Action<Message>? OnMessageReceived;

        public UdpCommunicator(Configuration configuration)
        {
            // Initialize UDP receiver and sender
            _udpReceiver = new UdpReceiver(configuration);
            _udpSender = new UdpSender(configuration);

            // Subscribe to the message received event in UdpReceiver
            _udpReceiver.OnMessageReceived += RaiseMessageReceivedEvent;
            _userName = configuration.Username;
        }

        public void Connect(CancellationToken cancellationToken)
        {
            // Start the UDP receiver to listen for incoming messages
            var listenTask = Task.Run(() => StartListening(cancellationToken));
            // Optionally, send a connection message upon connection
            var connectMessage = new Message(_userName, "Connecting to server", new TextMessage());
            SendMessage(connectMessage);
        }

        public void StartListening(CancellationToken cancellationToken)
        {
            // Start the UDP receiver to listen for incoming messages
            _udpReceiver.StartObservables(cancellationToken);
        }

        public void SendMessage(Message message)
        {
            _udpSender.SendMessage(message);  // Delegate the sending of the message to UdpSender
        }

        public Message ReceiveMessage()
        {
            return _udpReceiver.ReceiveMessage();  // Delegate receiving of the message to UdpReceiver
        }

        private void RaiseMessageReceivedEvent(Message message)
        {
            // Raise the OnMessageReceived event for the ChatClient to handle
            OnMessageReceived?.Invoke(message);
        }

        public void Dispose()
        {
            _udpSender.Dispose();
        }

        public void Stop()
        {
            _udpReceiver.Stop();
        }
    }
}