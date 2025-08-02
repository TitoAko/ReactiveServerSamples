using Microsoft.Extensions.Configuration;

namespace CoreLibrary.Utilities
{
    public static class ConfigurationLoader
    {
        /// <summary>Builds <see cref="Configuration"/> from JSON → ENV → CLI.</summary>
        public static Configuration Load(string[] args, string basePath = ".")
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json",
                             optional: true, reloadOnChange: true)
                .AddEnvironmentVariables(prefix: "CHAT_");

            if (args is { Length: > 0 })
            {
                builder.AddCommandLine(args);                // works once package + using are present
            }

            return builder.Build().Get<Configuration>()!;
        }
    }
}