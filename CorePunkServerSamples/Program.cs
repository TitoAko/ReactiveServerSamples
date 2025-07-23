using ServerApp;

Console.WriteLine($"Args: {string.Join('|', args)}");
await new ServerAppInitializer(args).RunAsync();
