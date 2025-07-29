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

        public async Task AddClientAsync(Configuration configuration)
        {
            var communicator = CommunicatorFactory.Create(configuration);
            var clientConnection = new ClientConnection(communicator);
            _users.Add(clientConnection);

            clientConnection.Received += (_, message) => OnMessage(clientConnection, message);

            await clientConnection.StartAsync();
        }

        private async void OnMessage(ClientConnection sender, Message message)
        {
            if (message.Type == MessageType.Exit)
            {
                _users.Remove(sender.Id);
                return;
            }

            // broadcast
            foreach (var targetClient in _users.All.Where(clientConnection => clientConnection.Id != sender.Id))
            {
                try { await targetClient.SendAsync(message); }
                catch { /* swallow for now; cleanup on future exit */ }
            }
        }
    }
}