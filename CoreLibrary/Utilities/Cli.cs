namespace CoreLibrary.Utilities;

/// <summary>Super-lightweight CLI switch reader:  --key value</summary>
public sealed class Cli
{
    private readonly Dictionary<string, string> _map = new(StringComparer.OrdinalIgnoreCase);

    public Cli(string[] args)
    {
        for (int i = 0; i + 1 < args.Length; i += 2)
        {
            if (args[i].StartsWith("--"))
            {
                _map[args[i]] = args[i + 1];
            }
        }
    }

    public string Get(string key, string fallback)
    {
        return _map.TryGetValue(key, out var v) ? v : fallback;
    }

    public int Get(string key, int fallback)
    {
        return int.TryParse(Get(key, ""), out var n) ? n : fallback;
    }
}
