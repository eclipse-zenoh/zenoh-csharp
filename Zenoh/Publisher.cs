using System;
using System.Runtime.InteropServices;

namespace Zenoh;

public struct PublisherHandle
{
    internal int handle;
}

public class Publisher : IDisposable
{
    internal unsafe ZOwnedPublisher* ownedPublisher;
    internal readonly ZPublisherOptions options;
    internal readonly string keyexpr;
    private bool _disposed;

    public Publisher(string key) : this(key, CongestionControl.Block, Priority.RealTime)
    {
    }

    public Publisher(string key, CongestionControl control, Priority priority)
    {
        unsafe
        {
            keyexpr = key;
            _disposed = false;
            ownedPublisher = null;
            options.congestion_control = control;
            options.priority = priority;
        }
    }

    public void Dispose() => Dispose(true);

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        unsafe
        {
            Marshal.FreeHGlobal((nint)ownedPublisher);
        }

        _disposed = true;
    }
}