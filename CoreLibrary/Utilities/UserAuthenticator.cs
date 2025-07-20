
namespace CoreLibrary.Utilities
{
    /// <summary>
    /// Mock authentication service. Returns true for any credentials.
    /// </summary>
    public class UserAuthenticator
    {
        /// <summary>
        /// Always returns true. Extend this for real auth logic if needed.
        /// </summary>
        public static bool Authenticate(string username, string password) => true;
    }
}