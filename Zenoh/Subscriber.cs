#pragma warning disable CS8500

using System;
using System.Runtime.InteropServices;

namespace Zenoh;

public struct SubscriberHandle
{
    internal int handle;
}

public delegate void SubscriberCallback(Sample sample);

public class Subscriber : IDisposable
{
    internal readonly unsafe ZOwnedClosureSample* closureSample;
    internal unsafe ZOwnedSubscriber* ownedSubscriber;
    internal readonly string keyexpr;
    internal readonly ZSubscriberOptions options;
    private GCHandle _userCallbackGcHandle;
    private bool _disposed;

    public Subscriber(string key, SubscriberCallback userCallback)
        : this(key, userCallback, Reliability.Reliable)
    {
    }

    public Subscriber(string key, SubscriberCallback userCallback, Reliability reliability)
    {
        unsafe
        {
            keyexpr = key;
            _disposed = false;
            options.reliability = reliability;
            _userCallbackGcHandle = GCHandle.Alloc(userCallback);
            ZOwnedClosureSample ownedClosureSample = new ZOwnedClosureSample
            {
                context = (void*)GCHandle.ToIntPtr(_userCallbackGcHandle),
                call = Call,
                drop = null,
            };

            nint p = Marshal.AllocHGlobal(Marshal.SizeOf(ownedClosureSample));
            Marshal.StructureToPtr(ownedClosureSample, p, false);
            closureSample = (ZOwnedClosureSample*)p;
            ownedSubscriber = null;
        }
    }

    public void Dispose() => Dispose(true);

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        unsafe
        {
            Marshal.FreeHGlobal((nint)closureSample);
            Marshal.FreeHGlobal((nint)ownedSubscriber);
        }

        _userCallbackGcHandle.Free();
        _disposed = true;
    }

    private static unsafe void Call(ZSample* zSample, void* context)
    {
        GCHandle gch = GCHandle.FromIntPtr((nint)context);
        SubscriberCallback callback = (SubscriberCallback)gch.Target;
        Sample sample = new Sample(zSample);
        if (callback != null)
        {
            callback(sample);
        }
    }
}