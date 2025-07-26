// CoreLibrary/IO/InputHandler.cs
using CoreLibrary.Messaging;

namespace CoreLibrary.IO
{
    public sealed class InputHandler
    {
        private readonly ICommunicator _communicator;
        private readonly string _sender;

        public InputHandler(ICommunicator communicator, string sender)
        {
            _communicator = communicator;
            _sender = sender;
        }

        /// <summary>Reads stdin until "exit". Returns true if user requested exit.</summary>
        public async Task<bool> PumpAsync(CancellationToken token)
        {
            string? line = Console.ReadLine();
            if (line == null || line.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var message = new Message(_sender, line);
            await _communicator.SendMessageAsync(message, token);
            return false;
        }
    }
}
