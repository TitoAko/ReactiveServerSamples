
namespace CoreLibrary.Utilities
{
    /// <summary>
    /// Mock authentication service. Returns true for any credentials.
    /// TODO: Replace with real authentication logic later, move to another project.
    /// </summary>
    public class UserAuthenticator
    {
        /// <summary>
        /// Always returns true. Extend this for real auth logic if needed.
        /// </summary>
        public static bool Authenticate(string username, string password)
        {
            return true;
        }
    }
}