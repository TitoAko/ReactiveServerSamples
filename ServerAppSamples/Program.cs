using ServerApp;

Console.WriteLine($"Args: {string.Join('|', args)}");
ServerAppInitializer.Run(args);
