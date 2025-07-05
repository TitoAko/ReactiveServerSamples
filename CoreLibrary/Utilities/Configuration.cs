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
                    .AddJsonFile(filePath, optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables() // Add this to support Docker/env vars
                    .Build();
                foreach (System.Collections.DictionaryEntry de in Environment.GetEnvironmentVariables())
                {
                    Console.WriteLine($"{de.Key} = {de.Value}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load configuration file '{filePath}': {ex.Message}", ex);
            }

            // Validate required settings
            if (string.IsNullOrWhiteSpace(IpAddress))
            {
                throw new InvalidOperationException("AppConfig:IpAddress is missing or empty in configuration.");
            }
            if (Port == 0)
            {
                throw new InvalidOperationException("AppConfig:Port is missing or invalid in configuration.");
            }
            if (string.IsNullOrWhiteSpace(Username))
            {
                throw new InvalidOperationException("AppConfig:Username is missing or empty in configuration.");
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                throw new InvalidOperationException("AppConfig:Password is missing or empty in configuration.");
            }
            if (string.IsNullOrWhiteSpace(LogLevel))
            {
                throw new InvalidOperationException("Logging:LogLevel is missing or empty in configuration.");
            }
        }

        public string AppType =>
            Environment.GetEnvironmentVariable("APP_TYPE") ??
            _configuration.GetSection("AppConfig")["AppType"] ??
            "server";

        public string IpAddress =>
            Environment.GetEnvironmentVariable("APP_IPADDRESS") ??
            _configuration.GetSection("AppConfig")["IpAddress"] ??
            "127.0.0.1";

        public int Port
        {
            get
            {
                var envPort = Environment.GetEnvironmentVariable("APP_PORT");
                if (!string.IsNullOrWhiteSpace(envPort) && int.TryParse(envPort, out int port) && port > 0)
                    return port;

                var value = _configuration.GetSection("AppConfig")["Port"];
                if (int.TryParse(value, out port) && port > 0)
                    return port;

                throw new InvalidOperationException("AppConfig:Port is missing or invalid in configuration.");
            }
        }

        public string Username =>
            Environment.GetEnvironmentVariable("APP_USERNAME") ??
            _configuration.GetSection("AppConfig")["Username"] ??
            throw new InvalidOperationException("AppConfig:Username is missing or empty in configuration.");

        public string Password =>
            Environment.GetEnvironmentVariable("APP_PASSWORD") ??
            _configuration.GetSection("AppConfig")["Password"] ??
            throw new InvalidOperationException("AppConfig:Password is missing or empty in configuration.");

        public string Communicator =>
            Environment.GetEnvironmentVariable("APP_COMMUNICATOR") ??
            _configuration.GetSection("AppConfig")["Communicator"] ??
            throw new InvalidOperationException("AppConfig:Communicator is missing or empty in configuration.");

        public string LogLevel =>
            Environment.GetEnvironmentVariable("APP_LOGLEVEL") ??
            _configuration.GetSection("Logging")["LogLevel"] ??
            "Info";

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
    }
}