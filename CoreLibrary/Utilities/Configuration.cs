using Microsoft.Extensions.Configuration;

namespace CoreLibrary.Utilities
{
    public class Configuration
    {
        private readonly IConfiguration _configuration;

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
            if (string.IsNullOrWhiteSpace(ServerIpAddress))
                throw new InvalidOperationException("ServerConfig:IpAddress is missing or empty in configuration.");
            if (ServerPort == 0)
                throw new InvalidOperationException("ServerConfig:Port is missing or invalid in configuration.");
            if (string.IsNullOrWhiteSpace(LogLevel))
                throw new InvalidOperationException("Logging:LogLevel is missing or empty in configuration.");
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
                throw new InvalidOperationException($"Configuration value for '{section}:{key}' is missing, empty, or invalid (must be a non-negative integer).");
            return configValue;
        }

        // Server configuration properties (read-only)
        public string ServerIpAddress
        {
            get
            {
                var value = _configuration.GetSection("ServerConfig")["IpAddress"];
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException("ServerConfig:IpAddress is missing or empty in configuration.");
                return value;
            }
        }

        public int ServerPort
        {
            get
            {
                var value = _configuration.GetSection("ServerConfig")["Port"];
                if (!int.TryParse(value, out int port) || port <= 0)
                    throw new InvalidOperationException("ServerConfig:Port is missing or invalid in configuration.");
                return port;
            }
        }

        // Logging configuration property (read-only)
        public string LogLevel
        {
            get
            {
                var value = _configuration.GetSection("Logging")["LogLevel"];
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException("Logging:LogLevel is missing or empty in configuration.");
                return value;
            }
        }
    }
}