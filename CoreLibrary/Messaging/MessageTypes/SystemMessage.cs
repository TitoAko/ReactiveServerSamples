using CoreLibrary.Interfaces;

namespace CoreLibrary.Messaging.MessageTypes
{
    public class SystemMessage : IMessageType
    {
        public void ProcessMessage(string sender, string content)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[SYSTEM] {sender}: {content}");
            Console.ResetColor();
        }
    }
}
