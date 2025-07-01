//using ServerApp;

namespace ClientApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Initialize ClientAppInitializer and rely on its constructor for initialization
            using (var initializer = new ClientAppInitializer())
            {
                // InitializeClient is now called inside the constructor of ClientAppInitializer
                Console.WriteLine("Client initialized successfully.");
            }
        }
    }
}