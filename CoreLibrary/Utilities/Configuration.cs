using Microsoft.Extensions.Configuration;

namespace CoreLibrary.Utilities
{
    public class Configuration
    {
        public string Username { get; }
        public string Password { get; }
        public string IpAddress { get; }
        public int Port { get; }
        public string Communicator { get; }
        public string AppType { get; }

        public Configuration(string configFileName = "appsettings.json")
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configFileName, optional: false)
                .AddEnvironmentVariables() // Support for Docker/env variables
                .Build();

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
