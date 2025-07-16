using CoreLibrary.Messaging;

namespace CoreLibrary.Interfaces
{
    public interface ICommunicator : IDisposable
    {
        void SendMessage(Message message);  // Send a message
        Message ReceiveMessage();  // Receive a message
        void StartListening(CancellationToken cancellationToken);
        void Stop();
        void Connect(CancellationToken cancellationToken);
    }
}
