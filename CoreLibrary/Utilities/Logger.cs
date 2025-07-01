using CoreLibrary.Interfaces;

namespace CoreLibrary.Utilities
{
    public class Logger : ILogger
    {
        public void Log(string message)
        {
            // Log the message to the console
            Console.WriteLine($"[Log]: {message}");
        }
    }
}
