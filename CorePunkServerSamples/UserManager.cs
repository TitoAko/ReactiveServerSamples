using ServerApp.Models;

namespace ServerApp;

/// <summary>Tracks all connected clients in a threadsafe way.</summary>
public sealed class UserManager
{
    private readonly Dictionary<string, ClientConnection> _clients = new();
    private readonly object _lock = new();

    public IReadOnlyCollection<ClientConnection> All
    {
        get { lock (_lock) return _clients.Values.ToList(); }
    }

    public void Add(ClientConnection c)
    {
        lock (_lock) _clients[c.Id] = c;
    }

    public void Remove(string id)
    {
        lock (_lock)
        {
            if (_clients.Remove(id, out var c))
                c.Dispose();
        }
    }
}
