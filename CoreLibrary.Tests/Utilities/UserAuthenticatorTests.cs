using CoreLibrary.Utilities;

using FluentAssertions;

namespace CoreLibrary.Tests.Utilities
{
    public class UserAuthenticatorTests
    {
        [Theory]
        [InlineData("user", "pass")]
        [InlineData("", "")]
        [InlineData("admin", "123456")]
        public void Authenticate_AlwaysReturnsTrue_ForAnyInput(string username, string password)
        {
            var result = UserAuthenticator.Authenticate(username, password);

            result.Should().BeTrue();
        }

        [Fact]
        public void Authenticate_DoesNotThrow_OnNulls()
        {
            var act = () => UserAuthenticator.Authenticate(null!, null!);

            act.Should().NotThrow();
        }
    }
}