namespace ServerApp
{
    internal class Program
    {
        /// <summary>
        /// Entry point for the Server application. Initializes and starts a single instance of the server.
        /// </summary>
        public static void Main(string[] args)
        {
            new ServerAppInitializer().InitializeServer();
        }
    }
}
