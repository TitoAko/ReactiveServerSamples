//using ServerApp;

namespace ClientApp
{
    public class Program
    {
        /// <summary>
        /// Entry point for the Client application. Initializes and starts a single instance of the client
        /// for the configured username, IP address, and port.
        /// </summary>
        public static void Main(string[] args)
        {
            using (var initializer = new ClientAppInitializer())
            {
                Console.WriteLine("Client initialized successfully.");
            }
        }
    }
}