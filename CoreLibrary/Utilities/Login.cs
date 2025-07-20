namespace CoreLibrary.Utilities
{
    /// <summary>
    /// Command-line login prompt that validates non-empty user input.
    /// </summary>
    public class Login
    {
        /// <summary>
        /// Prompts the user to enter valid credentials interactively.
        /// </summary>
        /// <returns>Tuple containing the username and password.</returns>
        public static (string username, string password)? GetUserCredentials()
        {
            Console.WriteLine("Enter Username:");
            string? username = Console.ReadLine();
            while (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("Username cannot be empty. Please enter a valid username:");
                username = Console.ReadLine();
            }

            Console.WriteLine("Enter Password:");
            string? password = Console.ReadLine();
            while (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Password cannot be empty. Please enter a valid password:");
                password = Console.ReadLine();
            }

            return (username, password);
        }
    }
}
