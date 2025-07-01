using CoreLibrary.Messaging;

namespace CoreLibrary.Interfaces
{
    public interface ICommunicator : IDisposable
    {
        void StartListening();  // Start listening for incoming messages
        void SendMessage(Message message);  // Send a message
        Message ReceiveMessage();  // Receive a message
        void StopListening();
    }
}
