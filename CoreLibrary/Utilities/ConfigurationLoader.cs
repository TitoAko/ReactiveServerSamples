using Microsoft.Extensions.Configuration;

namespace CoreLibrary.Utilities
{
    public static class ConfigurationLoader
    {
        /// <summary>Builds <see cref="Configuration"/> from JSON → ENV → CLI.</summary>
        public static Configuration Load(string[] args, string? basePath = null)
        {
            // Pick an absolute base path:
            // - if caller passed one: normalize it
            // - else: default to the app's base directory (bin/... at runtime)
            var root = string.IsNullOrWhiteSpace(basePath)
                ? AppContext.BaseDirectory
                : (Path.IsPathRooted(basePath) ? basePath : Path.GetFullPath(basePath));

            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

            var builder = new ConfigurationBuilder()
                .SetBasePath(root)
                // Make JSON optional so missing files don't crash tools/tests
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables(prefix: "CHAT_")
                .AddCommandLine(args);

            return builder.Build().Get<Configuration>()!;
        }
    }
}
