namespace CoreLibrary.Interfaces
{
    public interface IMessageType
    {
        void ProcessMessage(string sender, string content);
    }
}
