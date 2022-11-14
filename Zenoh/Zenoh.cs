//
// Copyright (c) 2021 ADLINK Technology Inc.
//
// This program and the accompanying materials are made available under the
// terms of the Eclipse Public License 2.0 which is available at
// http://www.eclipse.org/legal/epl-2.0, or the Apache License, Version 2.0
// which is available at https://www.apache.org/licenses/LICENSE-2.0.
//
// SPDX-License-Identifier: EPL-2.0 OR Apache-2.0
//
// Contributors:
//   ADLINK zenoh team, <zenoh@adlink-labs.tech>
//

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;


namespace Zenoh
{
    public static class Zenoh
    {
        public const string DllName = "zenohc";

        [DllImport(Zenoh.DllName, EntryPoint = "zc_init_logger")]
        public static extern void InitLogger();

        public static string IdBytesToStr(byte[] buf)
        {
            StringBuilder str = new StringBuilder();
            for (int i = buf.Length - 1; i >= 0; i--)
            {
                str.Append($"{buf[i]:X2}");
            }

            return str.ToString();
        }

        internal const int RoutersNum = 128;
        internal const int PeersNum = 256;
        internal const int IdLength = 16;
    }

    public enum ZEncodingPrefix : int // z_encoding_prefix_t
    {
        Empty = 0,
        AppOctetStream = 1,
        AppCustom = 2,
        TextPlain = 3,
        AppProperties = 4,
        AppJson = 5,
        AppSql = 6,
        AppInteger = 7,
        AppFloat = 8,
        AppXml = 9,
        AppXhtmlXml = 10,
        AppXWwwFormUrlencoded = 11,
        TextJson = 12,
        TextHtml = 13,
        TextXml = 14,
        TextCss = 15,
        TextCsv = 16,
        TextJavascript = 17,
        ImageJpeg = 18,
        ImagePng = 19,
        ImageGif = 20,
    }

    public enum ZSampleKind : int // z_sample_kind_t
    {
        Put = 0,
        Delete = 1,
    }

    public enum ZCongestionControl : int // z_congestion_control_t
    {
        Block,
        Drop,
    }

    public enum ZPriority : int // z_priority_t
    {
        RealTime = 1,
        InteractiveHigh = 2,
        InteractiveLow = 3,
        DataHigh = 4,
        Data = 5,
        DataLow = 6,
        Background = 7,
    }

    public enum ConsolidationMode : int // z_consolidation_mode_t
    {
        Full,
        Lazy,
        None,
    }

    public enum Reliability : int // z_reliability_t
    {
        BestEffort,
        Reliable,
    }

    public enum SubMode : int // z_submode_t
    {
        Push,
        Pull,
    }

    public enum ZTarget : int // z_target_t
    {
        BestMatching,
        All,
        None,
        AllComplete,
    }

    public enum QueryConsolidationTag // z_query_consolidation_t_Tag
    {
        Auto,
        Manual,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ZBytes // z_bytes_t, z_owned_bytes_t
    {
        public IntPtr start;
        public ulong len;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ZTimestamp // z_timestamp_t
    {
        public UInt64 time;
        public ZBytes id;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ZString // z_owned_string_t, z_string_t
    {
        internal IntPtr start;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ZEncoding // z_encoding_t
    {
        internal ZEncodingPrefix prefix;
        internal ZBytes suffix;

        public String PrefixToString()
        {
            return prefix.ToString();
        }

        public static ZEncoding New(ZEncodingPrefix prefix)
        {
            return FnZEncoding(prefix, IntPtr.Zero);
        }

        [DllImport(Zenoh.DllName, EntryPoint = "z_encoding", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ZEncoding FnZEncoding(ZEncodingPrefix prefix, IntPtr suffix);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Period // z_period_t
    {
        public uint origin;
        public uint period;
        public uint duration;

        public Period(uint origin, uint period, uint duration)
        {
            this.origin = origin;
            this.period = period;
            this.duration = duration;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Zid
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        internal byte[] id;

        public string ToStr()
        {
            return Zenoh.IdBytesToStr(id);
        }
    }

    public struct PutOptions
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct NativeType // z_put_options_t
        {
            internal ZEncoding encoding;
            internal ZCongestionControl congestionControl;
            internal ZPriority priority;
        }

        internal NativeType native;

        public PutOptions()
        {
            native = ZPutOptionsDefault();
        }

        public void SetEncoding(ZEncoding encoding)
        {
            native.encoding = encoding;
        }

        public void SetCongestionControl(ZCongestionControl congestionControl)
        {
            native.congestionControl = congestionControl;
        }

        public void SetPriority(ZPriority priority)
        {
            native.priority = priority;
        }

        [DllImport(Zenoh.DllName, EntryPoint = "z_put_options_default", CallingConvention = CallingConvention.Cdecl)]
        private static extern NativeType ZPutOptionsDefault();
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ZOwnedClosureZidCallback(ref Zid zid, IntPtr p);

    [StructLayout(LayoutKind.Sequential)]
    struct ZClosureZid // z_owned_closure_zid_t
    {
        internal IntPtr context;
        internal ZOwnedClosureZidCallback call;
        internal IntPtr drop;

        public ZClosureZid(ZOwnedClosureZidCallback call, IntPtr context)
        {
            this.context = context;
            this.call = call;
            this.drop = IntPtr.Zero;
        }
    }

    public class KeyExpr : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeType // z_keyexpr_t 
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            private UInt64[] _align;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            private UIntPtr[] _padding;

            internal string GetStr()
            {
                IntPtr ptr = ZKeyexprToString(this);
                string output = Marshal.PtrToStringAnsi(ptr);
                // should be free using libc free()
                Marshal.FreeHGlobal(ptr);
                return output;
            }
        }

        internal NativeType native;
        private IntPtr _key_buf;
        private bool _disposed;

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            Marshal.FreeHGlobal(this._key_buf);
            _disposed = true;
        }

        internal KeyExpr(NativeType native, IntPtr keyBuf)
        {
            this.native = native;
            this._key_buf = keyBuf;
            this._disposed = false;
        }

        public static KeyExpr FromString(string name)
        {
            IntPtr p = Marshal.StringToHGlobalAnsi(name);
            NativeType keyexpr = ZKeyexpr(p);
            //Marshal.FreeHGlobal(p);
            return new KeyExpr(keyexpr, p);
        }

        public string GetStr()
        {
            return native.GetStr();
        }

        [DllImport(Zenoh.DllName, EntryPoint = "z_keyexpr", CallingConvention = CallingConvention.Cdecl)]
        internal static extern NativeType ZKeyexpr(IntPtr name);

        [DllImport(Zenoh.DllName, EntryPoint = "z_keyexpr_to_string", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr ZKeyexprToString(NativeType keyexpr);
    }

    public class QueryTarget
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeType // z_query_target_t
        {
            internal uint kind;
            internal ZTarget target;
        }

        internal NativeType native;

        public ZTarget Target
        {
            get { return native.target; }
            set { native.target = value; }
        }

        public QueryTarget()
        {
            this.native = ZQueryTargetDefault();
        }

        [DllImport(Zenoh.DllName, EntryPoint = "z_query_target_default", CallingConvention = CallingConvention.Cdecl)]
        internal static extern NativeType ZQueryTargetDefault();
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct ConsolidationStrategy // z_consolidation_strategy_t
    {
        public ConsolidationMode firstRouters;
        public ConsolidationMode lastRouters;
        public ConsolidationMode reception;
    }

    public class QueryConsolidation
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeType // z_query_consolidation_t
        {
            public QueryConsolidationTag tag;
            public ConsolidationStrategy manual;
        }

        internal NativeType native;

        public QueryConsolidation()
        {
            this.native = ZQueryConsolidationDefault();
        }

        [DllImport(Zenoh.DllName, EntryPoint = "z_query_consolidation_default",
            CallingConvention = CallingConvention.Cdecl)]
        internal static extern QueryConsolidation.NativeType ZQueryConsolidationDefault();
    }

    public class Sample
    {
        //[StructLayout(LayoutKind.Explicit)]
        //[FieldOffset(0)]
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeType // z_owned_sample_t
        {
            internal KeyExpr.NativeType keyexpr;
            internal ZBytes payload;
            internal ZEncoding encoding;
            internal ZSampleKind kind;
            internal ZTimestamp timestamp;
        }

        public string Key { get; }
        public byte[] Value { get; }
        public ZEncoding Encoding { get; }

        internal Sample(NativeType sample)
        {
            Key = sample.keyexpr.GetStr();
            Value = ZTypes.ZBytesToBytesArray(sample.payload);
            Encoding = sample.encoding;
        }

        public string ValueToString()
        {
            string result = System.Text.Encoding.UTF8.GetString(Value, 0, Value.Length);
            return result;
        }
    }

    public class ReplayData
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeType // z_owned_reply_data_t
        {
            internal Sample.NativeType sample;
            internal uint sourceKind;
            internal ZBytes replierId;
        }

        public Sample Sample { get; }
        public uint SourceKind { get; }
        public byte[] ReplierID { get; }

        public ReplayData(NativeType native)
        {
            this.Sample = new Sample(native.sample);
            this.SourceKind = native.sourceKind;
            this.ReplierID = ZTypes.ZBytesToBytesArray(native.replierId);
        }
    }

    /*
    public class ReplayDataArray
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeType // z_owned_reply_data_array_t
        {
            internal IntPtr val;
            internal ulong len;
        }

        public List<ReplayData> List;

        public ReplayDataArray(NativeType native)
        {
            List = new List<ReplayData>();
            if (!ZReplyDataArrayCheck(ref native))
            {
                return;
            }

            unsafe
            {
                for (int i = 0; i < (int)native.len; i++)
                {
                    ReplayData.NativeType* p = (ReplayData.NativeType*)native.val +
                                               i * sizeof(ReplayData.NativeType);
                    ReplayData ele = new ReplayData(*p);
                    List.Add(ele);
                }
            }

            ZReplyDataArrayFree(ref native);
        }

        [DllImport(Zenoh.DllName, EntryPoint = "z_reply_data_array_check", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool ZReplyDataArrayCheck(ref NativeType replies);

        [DllImport(Zenoh.DllName, EntryPoint = "z_reply_data_array_free", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ZReplyDataArrayFree(ref NativeType replies);
    }
    */

    internal static class ZTypes
    {
        internal static byte[] ZBytesToBytesArray(ZBytes zb)
        {
            byte[] managedArray = new byte[(int)zb.len];
            System.Runtime.InteropServices.Marshal.Copy(zb.start, managedArray, 0, (int)zb.len);
            return managedArray;
        }

        internal static string ZBytesToString(ZBytes zb)
        {
            byte[] managedArray = new byte[(int)zb.len];
            System.Runtime.InteropServices.Marshal.Copy(zb.start, managedArray, 0, (int)zb.len);
            string result = System.Text.Encoding.UTF8.GetString(managedArray, 0, (int)zb.len);
            return result;
        }

        internal static string ZStringToString(ZString zs)
        {
            return Marshal.PtrToStringAnsi(zs.start);
        }

        [DllImport(Zenoh.DllName, EntryPoint = "z_string_free", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ZStringFree(ref ZString s);

        [DllImport(Zenoh.DllName, EntryPoint = "z_string_check", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool ZStringCheck(ref ZString s);

        [DllImport(Zenoh.DllName, EntryPoint = "z_bytes_free", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ZBytesFree(ref ZString s);
    }
}