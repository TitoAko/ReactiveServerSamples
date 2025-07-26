using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;

namespace CoreLibrary.Tests.Extra
{

    public class ClientConnectionDisposalTests
    {
        [Fact]
        public async Task Dispose_CancelsCommunicator()
        {
            var communicator = new FakeCommunicator();          // ← use stub
            communicator.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() =>
                communicator.SendMessageAsync(new Message("cli", "ping")));
        }
    }
}
