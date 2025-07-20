using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;

namespace ServerApp
{
    /// <summary>
    /// Handles message processing and broadcasting to connected clients.
    /// </summary>
    public class ChatServer : IBroadcastMessage, IMessageProcessor
    {
        private readonly UserManager _userManager;

        /// <summary>
        /// Initializes the chat server with a user manager.
        /// </summary>
        public ChatServer(UserManager userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Processes incoming messages and broadcasts them to all clients.
        /// </summary>
        public async Task ProcessAsync(Message message)
        {
            Console.WriteLine($"[Server] Processing from {message.Sender}: {message.Content}");
            BroadcastMessage(message);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Sends the given message to all connected clients.
        /// </summary>
        public void BroadcastMessage(Message message)
        {
            // Send a message to all connected clients
            foreach (var client in _userManager.GetAllClients())
            {
                client.SendMessage(message);
            }
        }
    }
}
