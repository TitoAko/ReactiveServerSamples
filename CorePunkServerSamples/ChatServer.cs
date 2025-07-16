using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;

namespace ServerApp
{
    public class ChatServer : IBroadcastMessage, IMessageProcessor
    {
        private readonly UserManager _userManager;

        public ChatServer(UserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task ProcessAsync(Message message)
        {
            Console.WriteLine($"[Server] Processing from {message.Sender}: {message.Content}");
            BroadcastMessage(message);
            await Task.CompletedTask;
        }

        // Broadcasting the message to all clients
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
