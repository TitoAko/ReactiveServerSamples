using CoreLibrary.Messaging;

namespace CoreLibrary.Interfaces
{
    public interface ICommunicator : IDisposable
    {
        Task SendMessage(Message message, CancellationToken cancellationToken = default); // Send a message
        Message ReceiveMessage();  // Receive a message
        void StartListening(CancellationToken cancellationToken);
        void Stop();
        void Connect(CancellationToken cancellationToken);
    }
}
