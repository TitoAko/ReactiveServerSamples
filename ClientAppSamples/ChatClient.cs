using CoreLibrary.Interfaces;
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

        private bool _disposed;

        public ChatClient(ICommunicator comm, string clientId)
        {
            _comm = comm;
            _input = new InputHandler(comm, clientId);

            _comm.MessageReceived += (_, m) => _output.DisplayMessage(m);
        }

        public async Task RunAsync(bool nonInteractive = false)
        {
            await _comm.StartAsync(_cts.Token);     // start receive loop

            if (nonInteractive)
            {
                Console.WriteLine("Input is redirected; chat client will not be interactive.");
                return;
            }

            Console.WriteLine("Type…  ('exit' to quit)");

            while (!_cts.Token.IsCancellationRequested)
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
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            _cts.Cancel();
            _comm.DisposeAsync().AsTask().GetAwaiter().GetResult();
            _cts.Dispose();
        }
    }
}
