#pragma warning disable CS8500
#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Zenoh;

public class Session : IDisposable
{
    internal SortedDictionary<int, Subscriber> subscribers;
    private int _indexSubscriber = 1;
    private bool _disposed;
    private readonly unsafe ZOwnedSession* _session;

    private unsafe Session(ZOwnedSession* session)
    {
        subscribers = new SortedDictionary<int, Subscriber>();
        _disposed = false;
        _session = session;
    }

    public static Session? Open(Config config)
    {
        unsafe
        {
            ZOwnedSession session = ZenohC.z_open(config.ownedConfig);
            if (ZenohC.z_session_check(&session) != 1)
            {
                return null;
            }

            nint p = Marshal.AllocHGlobal(Marshal.SizeOf(session));
            Marshal.StructureToPtr(session, p, false);

            return new Session((ZOwnedSession*)p);
        }
    }


    public void Close()
    {
        if (_disposed) return;

        unsafe
        {
            foreach ((_, Subscriber subscriber) in subscribers)
            {
                ZenohC.z_undeclare_subscriber(subscriber.ownedSubscriber);
                Marshal.FreeHGlobal((nint)subscriber.ownedSubscriber);
                subscriber.ownedSubscriber = null;
            }

            subscribers.Clear();

            ZenohC.z_close(_session);
            Marshal.FreeHGlobal((nint)_session);
        }

        _disposed = true;
    }

    public void Dispose() => Dispose(true);

    private void Dispose(bool disposing)
    {
        Close();
    }


    // private static List<string> GetIdStrings(IntPtr buf)
    // {
    //     List<string> list = new List<string>();
    //     int len = Marshal.ReadByte(buf);
    //     for (int i = 0; i < len; i++)
    //     {
    //         // byte[] b = new byte[ZenohC.IdLength];
    //         // Marshal.Copy(buf + 1 + ZenohC.IdLength * i, b, 0, ZenohC.IdLength);
    //         // list.Add(ZenohC.IdBytesToStr(b));
    //     }
    //
    //     return list;
    // }

    // private static void InfoZidCallback(ref Zid zid, IntPtr buf)
    // {
    //     int i = Marshal.ReadByte(buf);
    //     if (i >= ZenohC.RoutersNum)
    //     {
    //         return;
    //     }
    //
    //     Marshal.Copy(zid.id, 0, buf + 1 + ZenohC.IdLength * i, ZenohC.IdLength);
    //     Marshal.WriteByte(buf, (byte)(i + 1));
    // }

    public struct Id
    {
        internal byte[] data;

        internal Id(ZId zid)
        {
            data = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                unsafe
                {
                    data[i] = zid.id[i];
                }
            }
        }

        public string ToStr()
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
            {
                sb.Append(b.ToString("x"));
            }

            return sb.ToString();
        }
    }

    public Id LocalId()
    {
        unsafe
        {
            ZSession session = ZenohC.z_session_loan(_session);
            ZId zid = ZenohC.z_info_zid(session);
            return new Id(zid);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    private unsafe struct ZIdBuffer
    {
        internal nuint count;
        internal fixed byte data[256 * 16];

        public ZIdBuffer()
        {
            count = 0;
        }

        internal void Add(ZId* zId)
        {
            if (count >= 256) return;
            for (nuint i = 0; i < 16; i++)
            {
                data[count * 16 + i] = zId->id[i];
            }

            count += 1;
        }

        internal Id[] ToIds()
        {
            Id[] ids = new Id[count];
            for (nuint i = 0; i < count; i++)
            {
                byte[] o = new byte[16];
                for (nuint j = 0; j < 16; j++)
                {
                    o[j] = data[i * 16 + j];
                }

                Id id = new Id
                {
                    data = o,
                };
                ids[i] = id;
            }

            return ids;
        }
    }

    internal static unsafe void z_id_call(ZId* zId, void* context)
    {
        ZIdBuffer* pIdBuffer = (ZIdBuffer*)context;
        pIdBuffer->Add(zId);
    }

    public Id[] RoutersId()
    {
        unsafe
        {
            ZSession session = ZenohC.z_session_loan(_session);
            nint pIdBuffer = Marshal.AllocHGlobal(Marshal.SizeOf<ZIdBuffer>());
            ZOwnedClosureZId ownedClosureZId = new ZOwnedClosureZId
            {
                context = (void*)pIdBuffer,
                call = z_id_call,
                drop = null,
            };
            nint pOwnedClosureZId = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedClosureZId>());
            Marshal.StructureToPtr(ownedClosureZId, pOwnedClosureZId, false);

            ZenohC.z_info_routers_zid(session, (ZOwnedClosureZId*)pOwnedClosureZId);

            ZIdBuffer zIdBuffer;
            zIdBuffer = (ZIdBuffer)Marshal.PtrToStructure(pIdBuffer, typeof(ZIdBuffer));
            Id[] ids = zIdBuffer.ToIds();

            Marshal.FreeHGlobal(pOwnedClosureZId);
            Marshal.FreeHGlobal(pIdBuffer);
            return ids;
        }
    }

    // public Id[] PeersId()
    // {
    // }

    public bool PutStr(string key, string value)
    {
        return PutStr(key, value, CongestionControl.Block, Priority.RealTime);
    }

    public bool PutStr(string key, string s, CongestionControl congestionControl, Priority priority)
    {
        unsafe
        {
            byte[] data = Encoding.UTF8.GetBytes(s);
            ZPutOptions options = new ZPutOptions
            {
                encoding = ZenohC.z_encoding(EncodingPrefix.TextPlain, null),
                congestionControl = congestionControl,
                priority = priority,
            };
            return _put(key, data, options);
        }
    }

    public bool PutJson(string key, string value)
    {
        return PutJson(key, value, CongestionControl.Block, Priority.RealTime);
    }

    public bool PutJson(string key, string s, CongestionControl congestionControl, Priority priority)
    {
        unsafe
        {
            byte[] data = Encoding.UTF8.GetBytes(s);
            ZPutOptions options = new ZPutOptions
            {
                encoding = ZenohC.z_encoding(EncodingPrefix.AppJson, null),
                congestionControl = congestionControl,
                priority = priority,
            };
            return _put(key, data, options);
        }
    }

    public bool PutInt(string key, long value)
    {
        return PutInt(key, value, CongestionControl.Block, Priority.RealTime);
    }

    public bool PutInt(string key, long value, CongestionControl congestionControl, Priority priority)
    {
        unsafe
        {
            string s = value.ToString("G");
            byte[] data = Encoding.UTF8.GetBytes(s);
            ZPutOptions options = new ZPutOptions
            {
                encoding = ZenohC.z_encoding(EncodingPrefix.AppInteger, null),
                congestionControl = congestionControl,
                priority = priority,
            };
            return _put(key, data, options);
        }
    }

    public bool PutFloat(string key, double value)
    {
        return PutFloat(key, value, CongestionControl.Block, Priority.RealTime);
    }

    public bool PutFloat(string key, double value, CongestionControl congestionControl, Priority priority)
    {
        unsafe
        {
            string s = value.ToString("G");
            byte[] data = Encoding.UTF8.GetBytes(s);
            ZPutOptions options = new ZPutOptions
            {
                encoding = ZenohC.z_encoding(EncodingPrefix.AppFloat, null),
                congestionControl = congestionControl,
                priority = priority,
            };
            return _put(key, data, options);
        }
    }

    private bool _put(string key, byte[] value, ZPutOptions options)
    {
        if (_disposed) return false;
        unsafe
        {
            nint pv = Marshal.AllocHGlobal(value.Length);
            nuint len = (nuint)value.Length;
            Marshal.Copy(value, 0, pv, value.Length);
            nint pKey = Marshal.StringToHGlobalAnsi(key);
            ZSession session = ZenohC.z_session_loan(_session);
            ZKeyexpr keyexpr = ZenohC.z_keyexpr((byte*)pKey);
            int r = ZenohC.z_put(session, keyexpr, (byte*)pv, len, &options);
            Marshal.FreeHGlobal(pv);
            Marshal.FreeHGlobal(pKey);
            return r == 0;
        }
    }

    public SubscriberHandle? RegisterSubscriber(Subscriber subscriber)
    {
        unsafe
        {
            if (subscriber.ownedSubscriber != null)
            {
                return null;
            }

            ZSession session = ZenohC.z_session_loan(_session);
            nint pKey = Marshal.StringToHGlobalAnsi(subscriber.keyexpr);
            ZKeyexpr keyexpr = ZenohC.z_keyexpr((byte*)pKey);
            ZSubscriberOptions options = new ZSubscriberOptions
            {
                reliability = subscriber.reliability,
            };
            nint pOptions = Marshal.AllocHGlobal(Marshal.SizeOf<ZSubscriberOptions>());
            Marshal.StructureToPtr(options, pOptions, false);
            ZOwnedSubscriber sub =
                ZenohC.z_declare_subscriber(session, keyexpr, subscriber.closureSample, (ZSubscriberOptions*)pOptions);
            Marshal.FreeHGlobal(pOptions);
            Marshal.FreeHGlobal(pKey);

            if (ZenohC.z_subscriber_check(&sub) != 1)
            {
                return null;
            }

            nint pOwnedSubscriber = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedSubscriber>());
            Marshal.StructureToPtr(sub, pOwnedSubscriber, false);
            subscriber.ownedSubscriber = (ZOwnedSubscriber*)pOwnedSubscriber;

            _indexSubscriber += 1;
            subscribers.Add(_indexSubscriber, subscriber);

            return new SubscriberHandle
            {
                handle = _indexSubscriber,
            };
        }
    }

    public void UnregisterSubscriber(SubscriberHandle handle)
    {
        UnregisterSubscriber(handle.handle);
    }

    private void UnregisterSubscriber(int handle)
    {
        if (subscribers.TryGetValue(handle, out Subscriber? subscriber))
        {
            unsafe
            {
                ZenohC.z_undeclare_subscriber(subscriber.ownedSubscriber);
                Marshal.FreeHGlobal((nint)subscriber.ownedSubscriber);
                subscriber.ownedSubscriber = null;
            }

            subscribers.Remove(handle);
        }
    }
}