using CoreLibrary.Utilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace CoreLibrary.Tests.Utilities
{
    public class AppLockTests
    {
        private IConfiguration CreateInMemoryConfig(string usernameSuffix)
        {
            var values = new Dictionary<string, string?>
            {
                ["AppConfig:Username"] = $"TestUser{usernameSuffix}",
                ["AppConfig:Password"] = "testpass",
                ["AppConfig:IpAddress"] = "127.0.0.1",
                ["AppConfig:Port"] = "9999",
                ["AppConfig:Communicator"] = "UdpCommunicator",
                ["AppConfig:AppType"] = "Server"
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();
        }

        [Fact]
        public void ShouldAllowFirstInstance()
        {
            var config = new Configuration(CreateInMemoryConfig("A"));
            var appLock = new AppLock();

            var result = appLock.IsInstanceRunning(config);

            Assert.False(result);
            appLock.ReleaseLock();
        }

        [Fact]
        public void ShouldBlockSecondInstance()
        {
            var config = new Configuration(CreateInMemoryConfig("B"));
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
            var config = new Configuration(CreateInMemoryConfig("C"));
            var appLock = new AppLock();

            var first = appLock.IsInstanceRunning(config);
            appLock.ReleaseLock();

            var appLock2 = new AppLock();
            var second = appLock2.IsInstanceRunning(config);

            Assert.False(first);
            Assert.False(second);

            appLock2.ReleaseLock();
        }
    }
}
