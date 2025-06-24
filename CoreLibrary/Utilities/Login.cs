namespace CoreLibrary.Utilities
{
    public class Login
    {
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
