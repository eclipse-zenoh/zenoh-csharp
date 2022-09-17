using System;
using System.Runtime.InteropServices;

namespace Zenoh
{
    public delegate void SubscriberCallback(Sample sample);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void
        SubscriberCallbackNative(ref Sample.NativeType samplePtr, IntPtr arg);

    public class Subscriber
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeType // z_owned_subscriber_t
        {
            public IntPtr p;
        }

        internal KeyExpr key;
        internal NativeType nativeSubscriber;
        internal SubscriberCallback userCallback;
        internal SubscriberCallbackNative nativeCallback;

        public Subscriber(string key, SubscriberCallback userCallback)
        {
            this.key = KeyExpr.FromString(key);
            this.nativeSubscriber.p = IntPtr.Zero;
            this.userCallback = userCallback;
            this.nativeCallback = new SubscriberCallbackNative(SubscriberNativeCallbackImpl);
        }

        internal void SubscriberNativeCallbackImpl(ref Sample.NativeType samplePtr, IntPtr arg)
        {
            Sample s = new Sample(samplePtr);
            this.userCallback(s);
        }
    }
}