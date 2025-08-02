using CoreLibrary.Tests.TestInfrastructure;
using CoreLibrary.Utilities;

namespace CoreLibrary.Tests.EdgeCases
{
    public class AppLockReentryTests
    {
        [Fact]
        public void OnlyOneInstancePerRoleAndPort()
        {
            var cfg = TestConfig.TcpLoopback(12345);

            using var first = new AppLock(cfg);
            Assert.False(first.IsInstanceRunning);   // we are the first instance

            using var second = new AppLock(cfg);
            Assert.True(second.IsInstanceRunning);   // detects prior holder
        }
    }
}