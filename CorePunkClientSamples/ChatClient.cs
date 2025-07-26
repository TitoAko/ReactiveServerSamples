using CoreLibrary.IO;
using CoreLibrary.Messaging;

namespace ClientApp
{
    public sealed class ChatClient : IDisposable
    {
        private readonly ICommunicator _comm;
        private readonly InputHandler _input;
        private readonly OutputHandler _output = new();
        private readonly CancellationTokenSource _cts = new();

        public ChatClient(ICommunicator comm, string clientId)
        {
            _comm = comm;
            _input = new InputHandler(comm, clientId);

            _comm.MessageReceived += (_, m) => _output.DisplayMessage(m);
        }

        public async Task RunAsync()
        {
            _ = _comm.StartAsync(_cts.Token);     // start receive loop

            Console.WriteLine("Type…  ('exit' to quit)");

            while (true)
            {
                bool wantExit = await _input.PumpAsync(_cts.Token);
                if (wantExit)
                {
                    break;
                }
            }

            await _comm.SendMessageAsync(
                new Message(Environment.UserName, "<left chat>", MessageType.Exit),
                _cts.Token);

            Dispose();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _comm.Dispose();
        }
    }
}
