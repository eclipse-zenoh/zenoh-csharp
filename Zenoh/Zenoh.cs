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


namespace Zenoh
{
    public static class Zenoh
    {
        public const string DllName = "zenohc";

        [DllImport(Zenoh.DllName, EntryPoint = "z_init_logger")]
        public static extern void InitLogger();
    }

    public enum KnownEncoding : int // e_known_encoding_t
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
    public struct ZString // z_owned_string_t, z_string_t
    {
        internal IntPtr start;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ZEncoding // z_encoding_t
    {
        internal KnownEncoding prefix;
        internal ZBytes suffix;

        public String PrefixToString()
        {
            return prefix.ToString();
        }
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

    public class Info : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeType // z_owned_info_t
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            internal UInt64[] align;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            internal UIntPtr[] pad;
        }

        internal NativeType native;
        private bool _disposed;

        internal Info(NativeType natvie)
        {
            native = natvie;
            _disposed = false;
        }

        public void Dispose() => Dispose(true);

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            ZInfoFree(ref native);
            _disposed = true;
        }

        public String Pid()
        {
            uint key = 0x00;
            return InfoGet(key);
        }

        public String[] PeerPid()
        {
            uint key = 0x01;
            string r = InfoGet(key);
            return r.Split(',');
        }

        public String[] RouterPid()
        {
            uint key = 0x02;
            string r = InfoGet(key);
            return r.Split(',');
        }

        private String InfoGet(uint key)
        {
            ZString s = ZInfoGet(ref native, key);
            string result = ZTypes.ZStringToString(s);
            ZTypes.ZStringFree(ref s);
            return result;
        }

        [DllImport(Zenoh.DllName, EntryPoint = "z_info_free", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ZInfoFree(ref NativeType info);

        [DllImport(Zenoh.DllName, EntryPoint = "z_info_check", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool ZInfoCheck(ref NativeType info);

        [DllImport(Zenoh.DllName, EntryPoint = "z_info_get", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ZString ZInfoGet(ref NativeType info, UInt64 key);
    }

    public class KeyExpr : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeType // z_keyexpr_t
        {
            internal ulong id;
            internal ZBytes suffix;
        }

        internal NativeType native;
        private bool _disposed = false;

        public ulong Id
        {
            get { return native.id; }
        }

        public string Suffix
        {
            get { return Marshal.PtrToStringAnsi(native.suffix.start, (int)native.suffix.len); }
        }

        internal KeyExpr(NativeType native)
        {
            this.native = native;
        }

        public static KeyExpr FromString(string name)
        {
            IntPtr p = Marshal.StringToHGlobalAnsi(name);
            NativeType key = ZExprNew(p);
            Marshal.FreeHGlobal(p);
            return new KeyExpr(key);
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            ZKeyexprFree(ref native);
            _disposed = true;
        }

        [DllImport(Zenoh.DllName, EntryPoint = "z_expr_new", CallingConvention = CallingConvention.Cdecl)]
        internal static extern NativeType ZExprNew(IntPtr name);

        [DllImport(Zenoh.DllName, EntryPoint = "z_keyexpr_free", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ZKeyexprFree(ref NativeType key);
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
        [StructLayout(LayoutKind.Explicit)]
        public struct NativeType // z_owned_sample_t
        {
            [FieldOffset(0)] internal KeyExpr.NativeType key;
            [FieldOffset(24)] internal ZBytes value;
            [FieldOffset(48)] internal ZEncoding encoding;
        }

        public string Key { get; }
        public byte[] Value { get; }
        public ZEncoding Encoding { get; }

        internal Sample(NativeType sample)
        {
            Key = Marshal.PtrToStringAnsi(sample.key.suffix.start, (int)sample.key.suffix.len);
            Value = ZTypes.ZBytesToBytesArray(sample.value);
            Encoding = sample.encoding;
        }

        public string ValueToString()
        {
            string result = System.Text.Encoding.UTF8.GetString(Value, 0, Value.Length);
            return result;
        }

        [DllImport(Zenoh.DllName, EntryPoint = "z_sample_free", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ZSampleFree(ref NativeType sample);
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

    public class SubInfo
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeType // z_subinfo_t
        {
            public Reliability reliability;
            public SubMode mode;
            public Period period;
        }

        internal NativeType native;

        public SubInfo()
        {
            this.native = ZSubInfoDefault();
        }

        public SubInfo(SubMode mode) : this()
        {
            this.native.mode = mode;
        }

        public SubInfo(Reliability reliability, SubMode mode) : this()
        {
            native.reliability = reliability;
            native.mode = mode;
        }

        [DllImport(Zenoh.DllName, EntryPoint = "z_subinfo_default", CallingConvention = CallingConvention.Cdecl)]
        internal static extern NativeType ZSubInfoDefault();
    }


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