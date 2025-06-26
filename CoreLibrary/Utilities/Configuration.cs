using Microsoft.Extensions.Configuration;

namespace CoreLibrary.Utilities
{
    public class Configuration
    {
        private readonly IConfiguration _configuration;
        private string? _appType;

        public Configuration(string filePath)
        {
            try
            {
                _configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile(filePath, optional: false, reloadOnChange: true)
                    .Build();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load configuration file '{filePath}': {ex.Message}", ex);
            }

            // Validate required settings
            if (string.IsNullOrWhiteSpace(IpAddress))
            {
                throw new InvalidOperationException("ServerConfig:IpAddress is missing or empty in configuration.");
            }
            if (Port == 0)
            {
                throw new InvalidOperationException("ServerConfig:Port is missing or invalid in configuration.");
            }
            if (string.IsNullOrWhiteSpace(Username))
            {
                throw new InvalidOperationException("ServerConfig:Username is missing or empty in configuration.");
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                throw new InvalidOperationException("ServerConfig:Password is missing or empty in configuration.");
            }
            if (string.IsNullOrWhiteSpace(LogLevel))
            {
                throw new InvalidOperationException("Logging:LogLevel is missing or empty in configuration.");
            }
        }

        // Get the application type (server or client)
        public string? AppType
        {
            get => _appType;
        }

        // Get a configuration value as a string
        public string? GetConfigurationValueString(string section, string key)
        {
            return _configuration.GetSection(section)[key];
        }

        // Get an integer configuration value
        public int GetConfigurationValueInt(string section, string key)
        {
            var value = _configuration.GetSection(section)[key];
            if (string.IsNullOrWhiteSpace(value) || !int.TryParse(value, out int configValue) || configValue < 0)
            {
                throw new InvalidOperationException($"Configuration value for '{section}:{key}' is missing, empty, or invalid (must be a non-negative integer).");
            }
            return configValue;
        }

        // Server configuration properties (read-only)
        public string IpAddress
        {
            get
            {
                _appType = "server"; // Set the app type to server
                var value = _configuration.GetSection("ServerConfig")["IpAddress"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    _appType = "client"; // Fallback to client if server IP is not set
                    // Fallback to ClientConfig if ServerConfig is not set
                    value = _configuration.GetSection("ClientConfig")["IpAddress"];
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new InvalidOperationException("ServerConfig:IpAddress is missing or empty in configuration.");
                    }
                }
                return value;
            }
        }

        public int Port
        {
            get
            {
                _appType = "server"; // Set the app type to server
                var value = _configuration.GetSection("ServerConfig")["Port"];
                if (!int.TryParse(value, out int port) || port <= 0)
                {
                    _appType = "client"; // Fallback to client if server port is not set
                    value = _configuration.GetSection("ClientConfig")["Port"];
                    if (!int.TryParse(value, out port) || port <= 0)
                    {
                        throw new InvalidOperationException("ServerConfig:Port is missing or invalid in configuration.");
                    }
                }
                return port;
            }
        }

        public string Username
        {
            get
            {
                _appType = "server"; // Set the app type to server
                var value = _configuration.GetSection("ServerConfig")["Username"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    _appType = "client"; // Fallback to client if server username is not set
                    // Fallback to ClientConfig if ServerConfig is not set
                    value = _configuration.GetSection("ClientConfig")["Username"];
                    throw new InvalidOperationException("ServerConfig:Username is missing or empty in configuration.");
                }
                return value;
            }
        }

        public string Password
        {
            get
            {
                _appType = "server"; // Set the app type to server
                var value = _configuration.GetSection("ServerConfig")["Password"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    _appType = "client"; // Fallback to client if server password is not set
                    // Fallback to ClientConfig if ServerConfig is not set
                    value = _configuration.GetSection("ClientConfig")["Password"];
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new InvalidOperationException("ServerConfig:Password is missing or empty in configuration.");
                    }
                }
                return value;
            }
        }

        // Logging configuration property (read-only)
        public string LogLevel
        {
            get
            {
                var value = _configuration.GetSection("Logging")["LogLevel"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new InvalidOperationException("Logging:LogLevel is missing or empty in configuration.");
                }
                return value;
            }
        }
    }
}