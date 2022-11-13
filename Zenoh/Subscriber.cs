using System;
using System.Runtime.InteropServices;

namespace Zenoh
{
    public delegate void SubscriberCallback(Sample sample);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ZOwnedClosureSampleCallback(ref Sample.NativeType sample, IntPtr context);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ZOwnedClosureSampleDrop(ref Sample.NativeType sample, IntPtr context);

    [StructLayout(LayoutKind.Sequential)]
    internal struct ZClosureSample // z_owned_closure_sample_t
    {
        internal IntPtr context;
        internal ZOwnedClosureSampleCallback call;
        internal ZOwnedClosureSampleDrop drop;

        internal ZClosureSample(ZOwnedClosureSampleCallback call, ZOwnedClosureSampleDrop drop, IntPtr context)
        {
            this.call = call;
            this.drop = drop;
            this.context = context;
        }
    }

    public class Subscriber
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeType // z_owned_subscriber_t
        {
            public IntPtr p;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Options // z_subscriber_options_t
        {
            public Reliability reliability;
        }

        internal KeyExpr key;
        internal NativeType nativeSubscriber;
        internal SubscriberCallback userCallback;
        internal ZClosureSample closure;

        public Subscriber(string key, SubscriberCallback userCallback)
        {
            this.key = KeyExpr.FromString(key);
            this.nativeSubscriber.p = IntPtr.Zero;
            this.userCallback = userCallback;
            ZOwnedClosureSampleCallback call = SubscriberNativeCallbackImpl;
            this.closure = new ZClosureSample(call, null, IntPtr.Zero);
        }

        internal void SubscriberNativeCallbackImpl(ref Sample.NativeType samplePtr, IntPtr arg)
        {
            Sample s = new Sample(samplePtr);
            this.userCallback(s);
        }

        public static Options GetOptionsDefault()
        {
            return ZSubScriberOptionsDefault();
        }

        [DllImport(Zenoh.DllName, EntryPoint = "z_subscriber_options_default")]
        internal static extern Options ZSubScriberOptionsDefault();
    }
}