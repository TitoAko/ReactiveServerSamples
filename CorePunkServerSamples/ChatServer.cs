using CoreLibrary.Factories;
using CoreLibrary.Messaging;
using CoreLibrary.Utilities;
using ServerApp.Models;

namespace ServerApp
{
    /// <summary>
    /// Accepts new communicators, wires them into ClientConnections, and
    /// broadcasts messages to everyone else.
    /// </summary>
    public sealed class ChatServer
    {
        private readonly UserManager _users = new();

        public async Task AddClientAsync(Configuration cfg)
        {
            var comm = CommunicatorFactory.Create(cfg);
            var conn = new ClientConnection(comm);
            _users.Add(conn);

            conn.Received += (_, m) => OnMessage(conn, m);

            await conn.StartAsync();
        }

        private async void OnMessage(ClientConnection sender, Message m)
        {
            if (m.Type == MessageType.Exit)
            {
                _users.Remove(sender.Id);
                return;
            }

            // broadcast
            foreach (var other in _users.All.Where(c => c.Id != sender.Id))
            {
                try { await other.SendAsync(m); }
                catch { /* swallow for now; cleanup on future exit */ }
            }
        }
    }
}