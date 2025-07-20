using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Messaging.MessageTypes;

namespace ClientApp
{
    /// <summary>
    /// Handles user input interaction, including message parsing and exit control.
    /// </summary>
    public class InputHandler(IClient client, string username)
    {
        private static bool IsExit(string? input) =>
            string.Equals(input?.Trim(), "exit", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Prompts the user for input and returns the string entered.
        /// </summary>
        public string? GetUserInput()
        {
            Console.Write("Enter message: ");
            string? input = Console.ReadLine();
            return input;
        }

        /// <summary>
        /// Handles the full interactive loop for user input.
        /// </summary>
        public void HandleUserInput()
        {
            Console.WriteLine($"Welcome, {username}! Type 'exit' to quit.");
            while (true)
            {
                Console.WriteLine("Waiting for user input...");
                Console.SetCursorPosition(0, Console.CursorTop);

                string? input = GetUserInput();
                if (!string.IsNullOrEmpty(input))
                {
                    var (shouldExit, message) = Parse(input, username);

                    if (shouldExit)
                        break;

                    if (message is not null)
                        client.SendMessage(message);
                }
            }
        }

        /// <summary>
        /// Parses a raw input string and returns either an exit flag or a constructed message.
        /// </summary>
        /// <param name="input">The raw input text from the user.</param>
        /// <param name="username">The sender's username to attach to the message.</param>
        /// <returns>A tuple indicating whether the command is an exit and the parsed message object.</returns>
        public static (bool ShouldExit, Message? Message) Parse(string? input, string username)
        {
            if (string.IsNullOrWhiteSpace(input))
                return (false, null);

            if (string.Equals(input.Trim(), "exit", StringComparison.OrdinalIgnoreCase))
                return (true, null);

            return (false, new Message(username, input, new TextMessage()));
        }
    }
}
