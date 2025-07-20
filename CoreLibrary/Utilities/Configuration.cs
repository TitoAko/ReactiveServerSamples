using Microsoft.Extensions.Configuration;

namespace CoreLibrary.Utilities
{
    /// <summary>
    /// Loads and validates the application's configuration from appsettings.json and environment variables.
    /// </summary>
    public class Configuration
    {
        /// <summary>Gets the username for authentication.</summary>
        public string Username { get; }

        /// <summary>Gets the user password.</summary>
        public string Password { get; }

        /// <summary>Gets the IP address to bind or connect to.</summary>
        public string IpAddress { get; }

        /// <summary>Gets the port used for communication.</summary>
        public int Port { get; }

        /// <summary>Gets the chosen communication protocol (e.g., UdpCommunicator).</summary>
        public string Communicator { get; }

        /// <summary>Gets the application type (e.g., Client, Server).</summary>
        public string AppType { get; }

        /// <summary>
        /// Initializes configuration from the specified file (defaults to appsettings.json).
        /// </summary>
        public Configuration(string configFileName = "appsettings.json")
                : this(new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile(configFileName, optional: false)
                  .AddEnvironmentVariables()
                  .Build())
        {
        }

        /// <summary>
        /// Initializes configuration from a provided IConfiguration instance (e.g., for testing).
        /// </summary>
        public Configuration(IConfiguration config)
        {
            var section = config.GetSection("AppConfig");

            Username = section["Username"] ?? throw new InvalidOperationException("Missing Username");
            Password = section["Password"] ?? throw new InvalidOperationException("Missing Password");
            IpAddress = section["IpAddress"] ?? throw new InvalidOperationException("Missing IP Address");
            Communicator = section["Communicator"] ?? throw new InvalidOperationException("Missing Communicator");
            AppType = section["AppType"] ?? throw new InvalidOperationException("Missing AppType");

            if (!int.TryParse(section["Port"], out var port))
                throw new InvalidOperationException("Port is missing or invalid in configuration.");

            Port = port;
        }
    }
}
