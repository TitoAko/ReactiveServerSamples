using CoreLibrary.Interfaces;

namespace ServerApp
{
    public class UserManager
    {
        private readonly List<IClient> _clients = new List<IClient>();

        public void AddClient(IClient client)
        {
            _clients.Add(client);
        }

        public void RemoveClient(IClient client)
        {
            _clients.Remove(client);
        }

        public IEnumerable<IClient> GetAllClients()
        {
            return _clients;
        }
    }
}
