using CoreLibrary.Interfaces;

namespace ServerApp
{
    /// <summary>
    /// Manages a list of connected clients on the server.
    /// </summary>
    public class UserManager
    {
        private readonly List<IClient> _clients = new List<IClient>();

        /// <summary>
        /// Adds a new client to the user list.
        /// </summary>
        public void AddClient(IClient client)
        {
            _clients.Add(client);
        }

        /// <summary>
        /// Removes a client from the user list.
        /// </summary>
        public void RemoveClient(IClient client)
        {
            _clients.Remove(client);
        }

        /// <summary>
        /// Retrieves all currently connected clients.
        /// </summary>
        public IEnumerable<IClient> GetAllClients()
        {
            return _clients;
        }
    }
}
