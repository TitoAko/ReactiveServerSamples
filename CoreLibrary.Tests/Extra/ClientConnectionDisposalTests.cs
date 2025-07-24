using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;
using ServerApp.Models;

namespace CoreLibrary.Tests.Extra
{

    public class ClientConnectionDisposalTests
    {
        [Fact]
        public async Task Dispose_CancelsCommunicator()
        {
            var comm = new FakeCommunicator();          // ← use stub
            comm.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() =>
                comm.SendMessageAsync(new Message("cli", "ping")));
        }
    }
}
