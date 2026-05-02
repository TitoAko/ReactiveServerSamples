using CoreLibrary.IO;
using CoreLibrary.Tests.TestInfrastructure;

using FluentAssertions;

namespace CoreLibrary.Tests.IO
{
    public class InputHandlerTests
    {
        [Fact]
        public async Task PumpAsync_Exit_ReturnsTrue()
        {
            TextReader originalIn = Console.In;

            try
            {
                var fake = new FakeCommunicator();
                var input = new InputHandler(fake, "user");

                Console.SetIn(new StringReader("exit" + Environment.NewLine));

                var result = await input.PumpAsync(CancellationToken.None);

                result.Should().BeTrue();
                fake.SentMessages.Should().BeEmpty();
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }
    }
}
