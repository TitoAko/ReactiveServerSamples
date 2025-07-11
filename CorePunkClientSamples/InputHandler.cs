using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Messaging.MessageTypes;

namespace ClientApp
{
    public class InputHandler(IClient client, string username)
    {
        public string? GetUserInput()
        {
            Console.Write("Enter message: ");
            string? input = Console.ReadLine(); // Clear the console line before reading input
            return input;
        }

        /// <summary>
        /// Continuously handles user input and sends it when available
        /// </summary>
        public void HandleUserInput()
        {
            Console.WriteLine($"Welcome, {username}! Type 'exit' to quit.");
            while (true)
            {
                // control if it works
                Console.WriteLine("Waiting for user input...");
                // Clear the console line before reading input
                Console.SetCursorPosition(0, Console.CursorTop); // Move cursor to the beginning of the line

                // Get user input
                string? input = GetUserInput();
                if (!string.IsNullOrEmpty(input))
                {
                    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                    // Create a message with the input and send it to the server
                    var message = new Message(username, input, new TextMessage());
                    client.SendMessage(message);
                }
            }
        }
    }
}
