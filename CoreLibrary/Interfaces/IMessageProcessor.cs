using CoreLibrary.Messaging;

namespace CoreLibrary.Interfaces
{
    public interface IMessageProcessor
    {
        void ProcessMessage(Message message);
    }
}
