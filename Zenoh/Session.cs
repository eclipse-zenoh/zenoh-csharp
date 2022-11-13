using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

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

        public string LocalZid()
        {
            Zid zid = ZInfoZid(ref native);
            return zid.ToStr();
        }


        private static List<string> GetIdStrings(IntPtr buf)
        {
            List<string> list = new List<string>();
            int len = Marshal.ReadByte(buf);
            for (int i = 0; i < len; i++)
            {
                byte[] b = new byte[Zenoh.IdLength];
                Marshal.Copy(buf + 1 + Zenoh.IdLength * i, b, 0, Zenoh.IdLength);
                list.Add(Zenoh.IdBytesToStr(b));
            }

            return list;
        }

        private static void InfoZidCallback(ref Zid zid, IntPtr buf)
        {
            int i = Marshal.ReadByte(buf);
            if (i >= Zenoh.RoutersNum)
            {
                return;
            }

            Marshal.Copy(zid.id, 0, buf + 1 + Zenoh.IdLength * i, Zenoh.IdLength);
            Marshal.WriteByte(buf, (byte)(i + 1));
        }

        public string[] RoutersZid()
        {
            int length = 1 + Zenoh.IdLength * Zenoh.RoutersNum;
            IntPtr buffer = Marshal.AllocHGlobal(length);
            for (int i = 0; i < length; i++)
            {
                Marshal.WriteByte(buffer, i, 0);
            }

            ZClosureZid closure = new ZClosureZid(InfoZidCallback, buffer);
            sbyte r = ZInfoRoutersZid(ref native, ref closure);
            if (r != 0)
            {
                return Array.Empty<string>();
            }

            var output = GetIdStrings(buffer);

            Marshal.FreeHGlobal(buffer);
            return output.ToArray();
        }

        public string[] PeersZid()
        {
            int length = 1 + Zenoh.IdLength * Zenoh.PeersNum;
            IntPtr buffer = Marshal.AllocHGlobal(length);
            for (int i = 0; i < length; i++)
            {
                Marshal.WriteByte(buffer, i, 0);
            }

            ZClosureZid closure = new ZClosureZid(InfoZidCallback, buffer);
            sbyte r = ZInfoPeersZid(ref native, ref closure);
            if (r != 0)
            {
                return Array.Empty<string>();
            }

            var output = GetIdStrings(buffer);

            Marshal.FreeHGlobal(buffer);
            return output.ToArray();
        }

        public bool Put(KeyExpr key, string value, ref PutOptions options)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            IntPtr v = Marshal.AllocHGlobal(data.Length + 1);
            Marshal.Copy(data, 0, v, data.Length);
            Marshal.WriteByte(v, data.Length, 0);
            int r = ZPut(ref native, key.native, v, (ulong)data.Length, ref options.native);
            Marshal.FreeHGlobal(v);
            if (r == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /*
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
        */

        public bool RegisterSubscriber(Subscriber subscriber)
        {
            return RegisterSubscriber(subscriber, Subscriber.GetOptionsDefault());
        }

        public bool RegisterSubscriber(Subscriber subscriber, Subscriber.Options options)
        {
            Subscriber.NativeType nativeSubscriber = ZDeclareSubscribe(ref native, subscriber.key.native,
                ref subscriber.closure,
                ref options);
            if (ZSubscriberCheck(ref nativeSubscriber))
            {
                subscriber.nativeSubscriber = nativeSubscriber;
                Subscribers[subscriber.key.GetStr()] = subscriber;
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

        [DllImport(Zenoh.DllName, EntryPoint = "z_info_zid", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Zid ZInfoZid(ref NativeType session);

        [DllImport(Zenoh.DllName, EntryPoint = "z_info_peers_zid", CallingConvention = CallingConvention.Cdecl)]
        internal static extern sbyte ZInfoPeersZid(ref NativeType session, ref ZClosureZid callback);

        [DllImport(Zenoh.DllName, EntryPoint = "z_info_routers_zid", CallingConvention = CallingConvention.Cdecl)]
        internal static extern sbyte ZInfoRoutersZid(ref NativeType session, ref ZClosureZid callback);

        [DllImport(Zenoh.DllName, EntryPoint = "z_put", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ZPut(ref NativeType session, KeyExpr.NativeType keyexpr, IntPtr payload, ulong len,
            ref PutOptions.NativeType opts);

        [DllImport(Zenoh.DllName, EntryPoint = "z_subscriber_check")]
        internal static extern bool ZSubscriberCheck(ref Subscriber.NativeType sub);

        [DllImport(Zenoh.DllName, EntryPoint = "z_declare_subscriber")]
        internal static extern Subscriber.NativeType ZDeclareSubscribe(ref NativeType session,
            KeyExpr.NativeType keyexpr,
            ref ZClosureSample callback, ref Subscriber.Options opts);

        [DllImport(Zenoh.DllName, EntryPoint = "z_subscriber_close")]
        internal static extern void ZSubscriberClose(ref Subscriber.NativeType sub);

        /*
        [DllImport(Zenoh.DllName, EntryPoint = "z_get_collect", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ReplayDataArray.NativeType ZGetCollect(ref NativeType session,
            KeyExpr.NativeType keyexpr, IntPtr predicate, QueryTarget.NativeType target,
            QueryConsolidation.NativeType consolidation);
        */
    }
}