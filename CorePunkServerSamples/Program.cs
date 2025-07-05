using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;

namespace ServerApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new ServerAppInitializer().StartObservables();
        }
    }
}
