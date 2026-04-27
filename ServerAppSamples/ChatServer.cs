using CoreLibrary.Factories;
using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

using ServerApp.Models;

namespace ServerApp
{
    /// <summary>
    /// Manages client connections and fan-out broadcasting.
    /// </summary>
    public sealed class ChatServer
    {
        private readonly UserManager _users = new();

        public void AddClient(Configuration configuration)
        {
            var communicator = CommunicatorFactory.Create(configuration);
            AddClient(communicator);
        }

        internal void AddClient(ICommunicator communicator)
        {
            var clientConnection = new ClientConnection(communicator);

            _users.Add(clientConnection);
            clientConnection.Received += OnMessageAsync;

            _ = clientConnection.StartAsync().ContinueWith(t =>
            {
                if (t.Exception is not null)
                {
                    Console.Error.WriteLine(t.Exception);
                }
            });
        }

        private async void OnMessageAsync(object? sender, Message message)
        {
            if (sender is not ClientConnection from)
            {
                return;
            }

            if (message.Type == MessageType.Exit)
            {
                try
                {
                    await from.DisposeAsync();
                }
                catch
                {
                    // ignore for now, but TODO: structured logging
                }
                _users.Remove(from.Id);
                return;
            }

            var sendTasks = _users.All
                                          .Where(c => c.Id != from.Id)
                                          .Select(c => c.SendAsync(message));
            try { await Task.WhenAll(sendTasks); }
            catch { /* TODO: structured logging */ }
        }
    }
}