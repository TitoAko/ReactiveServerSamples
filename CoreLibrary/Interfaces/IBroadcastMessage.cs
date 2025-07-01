using CoreLibrary.Messaging;

namespace CoreLibrary.Interfaces
{
    public interface IBroadcastMessage
    {
        void BroadcastMessage(Message message);
    }
}
