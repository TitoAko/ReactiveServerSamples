using ClientApp;

Console.WriteLine($"Args: {string.Join('|', args)}");
await ClientAppInitializer.RunAsync(args);
