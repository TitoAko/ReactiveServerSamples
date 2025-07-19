using CoreLibrary.Utilities;
using Xunit;
using System;
using System.IO;

namespace CoreLibrary.Tests.Utilities
{
    public class AppLockTests : IDisposable
    {
        private readonly string _tempConfigPath;

        public AppLockTests()
        {
            _tempConfigPath = Path.GetTempFileName();
        }

        private Configuration CreateConfig(string usernameSuffix)
        {
            var configContent = $$"""
            {
              "Username": "TestUser{{usernameSuffix}}",
              "IpAddress": "127.0.0.1",
              "Port": 9999,
              "Communicator": "UdpCommunicator"
            }
            """;

            File.WriteAllText(_tempConfigPath, configContent);
            return new Configuration(_tempConfigPath);
        }

        [Fact]
        public void ShouldAllowFirstInstance()
        {
            var config = CreateConfig("A");
            var appLock = new AppLock();

            var result = appLock.IsInstanceRunning(config);

            Assert.False(result);
            appLock.ReleaseLock();
        }

        [Fact]
        public void ShouldBlockSecondInstance()
        {
            var config = CreateConfig("B");
            var appLock1 = new AppLock();
            var appLock2 = new AppLock();

            var first = appLock1.IsInstanceRunning(config);
            var second = appLock2.IsInstanceRunning(config);

            Assert.False(first);
            Assert.True(second);

            appLock1.ReleaseLock();
            appLock2.ReleaseLock();
        }

        [Fact]
        public void ShouldReleaseLockProperly()
        {
            var config = CreateConfig("C");
            var appLock = new AppLock();

            var first = appLock.IsInstanceRunning(config);
            appLock.ReleaseLock();

            var appLock2 = new AppLock();
            var second = appLock2.IsInstanceRunning(config);

            Assert.False(first);
            Assert.False(second);

            appLock2.ReleaseLock();
        }

        public void Dispose()
        {
            if (File.Exists(_tempConfigPath))
            {
                File.Delete(_tempConfigPath);
            }
        }
    }
}
