namespace CoreLibrary.Utilities;

/// <summary>Ensures only one process role+port instance per machine.</summary>
public sealed class AppLock : IDisposable
{
    private readonly Mutex _mutex;
    private readonly bool _createdNew;

    public AppLock(Configuration cfg)
    {
        var name = $"Chat-{cfg.Role}-{cfg.Port}";
        _mutex = new Mutex(initiallyOwned: true, name, out _createdNew);
    }

    /// <summary>True if *another* instance already held the lock.</summary>
    public bool IsInstanceRunning => !_createdNew;

    public void Dispose()
    {
        if (_createdNew)                // we own the handle
        {
            _mutex.ReleaseMutex();
        }

        _mutex.Dispose();
    }
}
