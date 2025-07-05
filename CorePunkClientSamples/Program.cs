//using ServerApp;

namespace ClientApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var initializer = new ClientAppInitializer())
            {
                Console.WriteLine("Client initialized successfully.");
            }
        }
    }
}