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
            return Console.ReadLine();
        }

        /// <summary>
        /// Continuously handles user input and sends it when available
        /// </summary>
        public void HandleUserInput()
        {
            while (true)
            {
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
