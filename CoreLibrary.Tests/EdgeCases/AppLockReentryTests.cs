using CoreLibrary.Utilities;
using Xunit;

namespace CoreLibrary.Tests.EdgeCases
{
    public class AppLockReentryTests
    {
        [Fact]
        public void SecondInstance_IsBlocked()
        {
            var cfg = new Configuration { Port = 12345 };   // static port

            var lock1 = new AppLock();
            Assert.False(lock1.IsInstanceRunning(cfg));

            var lock2 = new AppLock();
            Assert.True(lock2.IsInstanceRunning(cfg));

            lock1.ReleaseLock();
            lock2.ReleaseLock();
        }
    }
}