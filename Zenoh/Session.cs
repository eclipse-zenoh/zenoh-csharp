using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Zenoh
{
    public class Session : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct NativeType // z_owned_session_t
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public IntPtr[] p;
        }

        internal NativeType native;
        internal Dictionary<string, Subscriber> Subscribers;

        public Session()
        {
            this.native = new NativeType();
            this.Subscribers = new Dictionary<string, Subscriber>();
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (native.p[0] != IntPtr.Zero)
            {
                foreach (Subscriber s in Subscribers.Values)
                {
                    // free
                }

                Subscribers.Clear();

                ZClose(ref native);
                for (int i = 0; i < 3; i++)
                {
                    native.p[i] = IntPtr.Zero;
                }
            }
        }

        public bool Open(Config config)
        {
            NativeType s = ZOpen(ref config.native);
            if (ZSessionCheck(ref s))
            {
                this.native = s;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Close()
        {
            ZClose(ref native);
        }

        public bool Put(KeyExpr key, string value)
        {
            IntPtr v = Marshal.StringToHGlobalAnsi(value);
            int r = ZPut(ref native, key.native, v, (ulong)value.Length);
            Marshal.FreeHGlobal(v);
            if (r == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ReplayDataArray Get(KeyExpr key)
        {
            QueryTarget target = new QueryTarget();
            target.Target = ZTarget.All;
            QueryConsolidation consolidation = new QueryConsolidation();
            string v = "abc";
            IntPtr predicate = Marshal.StringToHGlobalAnsi(v);
            ReplayDataArray.NativeType nativeData =
                ZGetCollect(ref native, key.native, predicate, target.native, consolidation.native);
            Marshal.FreeHGlobal(predicate);
            return new ReplayDataArray(nativeData);
        }

        public bool RegisterSubscriber(SubInfo subInfo, Subscriber subscriber)
        {
            Subscriber.NativeType nativeSubscriber = ZSubscribe(ref native, subscriber.key.native, subInfo.native,
                subscriber.nativeCallback,
                IntPtr.Zero);
            if (ZSubscriberCheck(ref nativeSubscriber))
            {
                subscriber.nativeSubscriber = nativeSubscriber;
                Subscribers[subscriber.key.Suffix] = subscriber;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UnregisterSubscriber(string key)
        {
            Subscriber subscriber = Subscribers[key];
            // 目前执行这句会出问题, 还在解决中
            // zSubscriberClose(ref subscriber.nativeSubscriber);
            Subscribers.Remove(key);
        }

        [DllImport(Zenoh.DllName, EntryPoint = "z_open", CallingConvention = CallingConvention.Cdecl)]
        internal static extern NativeType ZOpen(ref Config.NativeType config);

        [DllImport(Zenoh.DllName, EntryPoint = "z_session_check", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool ZSessionCheck(ref NativeType session);

        [DllImport(Zenoh.DllName, EntryPoint = "z_close", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ZClose(ref NativeType session);

        [DllImport(Zenoh.DllName, EntryPoint = "z_put", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ZPut(ref NativeType session, KeyExpr.NativeType keyexpr, IntPtr payload, ulong len);

        [DllImport(Zenoh.DllName, EntryPoint = "z_subscriber_check")]
        internal static extern bool ZSubscriberCheck(ref Subscriber.NativeType sub);

        [DllImport(Zenoh.DllName, EntryPoint = "z_subscribe")]
        internal static extern Subscriber.NativeType ZSubscribe(ref NativeType session, KeyExpr.NativeType keyexpr,
            SubInfo.NativeType sub_info,
            SubscriberCallbackNative callback, IntPtr arg);

        [DllImport(Zenoh.DllName, EntryPoint = "z_subscriber_close")]
        internal static extern void ZSubscriberClose(ref Subscriber.NativeType sub);

        [DllImport(Zenoh.DllName, EntryPoint = "z_get_collect", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ReplayDataArray.NativeType ZGetCollect(ref NativeType session,
            KeyExpr.NativeType keyexpr, IntPtr predicate, QueryTarget.NativeType target,
            QueryConsolidation.NativeType consolidation);
    }
}