using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;
using ServerApp.Models;

namespace CoreLibrary.Tests.Extra
{

    public class ClientConnectionDisposalTests
    {
        [Fact]
        public void Dispose_CancelsCommunicator()
        {
            var fake = new FakeCommunicator();
            var conn = new ClientConnection(fake);

            conn.Dispose();
            Assert.ThrowsAnyAsync<Exception>(() => fake.SendMessageAsync(new Message("x", "y")));
        }
    }
}
