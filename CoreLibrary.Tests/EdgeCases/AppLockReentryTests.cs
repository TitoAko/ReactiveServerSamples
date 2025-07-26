using CoreLibrary.Utilities;

namespace CoreLibrary.Tests.EdgeCases
{
    public class AppLockReentryTests
    {
        [Fact]
        public void SecondInstance_IsBlocked()
        {
            var configuration = new Configuration { Port = 12345 };   // static port

            var lock1 = new AppLock();
            Assert.False(lock1.IsInstanceRunning(configuration));

            var lock2 = new AppLock();
            Assert.True(lock2.IsInstanceRunning(configuration));

            lock1.ReleaseLock();
            lock2.ReleaseLock();
        }
    }
}