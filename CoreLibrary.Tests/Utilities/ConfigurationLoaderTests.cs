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
}