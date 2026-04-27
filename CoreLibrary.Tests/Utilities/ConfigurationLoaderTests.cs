using CoreLibrary.IO;
using CoreLibrary.Tests.TestInfrastructure;
using CoreLibrary.Utilities;

using FluentAssertions;

namespace CoreLibrary.Tests.Utilities;

public class ConfigurationLoaderTests
{
    [Fact]
    public void Load_Defaults_Work()
    {
        var cfg = ConfigurationLoader.Load(Array.Empty<string>());

        cfg.Should().NotBeNull();
        cfg.ListenPort.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task PumpAsync_Exit_ReturnsTrue()
    {
        var fake = new FakeCommunicator();
        var input = new InputHandler(fake, "user");

        Console.SetIn(new StringReader("exit"));

        var result = await input.PumpAsync(CancellationToken.None);

        result.Should().BeTrue();
    }
}