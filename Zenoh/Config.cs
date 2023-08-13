using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Zenoh
{
    public class Config : IDisposable
    {
        public enum Mode
        {
            Router,
            Peer,
            Client,
        }

        internal struct NativeType // z_owned_config_t
        {
            internal IntPtr p;
        }

        internal NativeType native;
        private bool _disposed;

        public Config()
        {
            this.native = ZConfigDefault();
            _disposed = false;
        }

        public void Dispose() => Dispose(true);

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            ZConfigDrop(ref native);
            _disposed = true;
        }

        // The Zenoh ID (as hex-string) of the instance.
        // This ID cannot exceed 16 bytes (as hex-string 32 bytes) of length.
        // If left unset, a random UUIDv4 will be generated.
        // WARNING: this id must be unique in your zenoh network.
        public bool SetId(string id)
        {
            string key = "id";
            string value = $"\"{id}\"";
            return InsertJson(key, value);
        }

        // The v such as "tcp/172.30.1.1:7447"
        public bool SetConnect(string[] v)
        {
            string key = "connect/endpoints";
            string value = "[";
            foreach (var ele in v)
            {
                value += $"\"{ele}\",";
            }

            value += "]";
            return InsertJson(key, value);
        }
        
        // The v such as "tcp/127.0.0.1:7888"
        public bool SetListen(string[] v)
        {
            string key = "listen/endpoints";
            string value = "[";
            foreach (var ele in v)
            {
                value += $"\"{ele}\",";
            }

            value += "]";
            return InsertJson(key, value);
        }

        public bool SetMode(Mode mode)
        {
            string key = "mode";
            string value = "";
            switch (mode)
            {
                case Mode.Client:
                    value = "\"client\"";
                    break;
                case Mode.Peer:
                    value = "\"peer\"";
                    break;
                case Mode.Router:
                    value = "\"client\"";
                    break;
            }

            return InsertJson(key, value);
        }

        // Whether data messages should be timestamped
        public bool SetTimestamp(bool b)
        {
            string key = "add_timestamp";
            string value = b ? "true" : "false";
            return InsertJson(key, value);
        }

        public string ToStr()
        {
            IntPtr p = ZCConfigToString(ref native);
            string result = Marshal.PtrToStringAnsi(p);
            return result;
        }

        private bool InsertJson(string key, string value)
        {
            IntPtr k = Marshal.StringToHGlobalAnsi(key);
            IntPtr v = Marshal.StringToHGlobalAnsi(value);
            sbyte r = ZCConfigInsertJson(ref native, k, v);
            Marshal.FreeHGlobal(k);
            Marshal.FreeHGlobal(v);
            return r == 1 ? true : false;
        }

        [DllImport(ZenohC.DllName, EntryPoint = "z_config_default", CallingConvention = CallingConvention.Cdecl)]
        internal static extern NativeType ZConfigDefault();

        [DllImport(ZenohC.DllName, EntryPoint = "z_config_drop", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ZConfigDrop(ref NativeType config);

        [DllImport(ZenohC.DllName, EntryPoint = "zc_config_from_str", CallingConvention = CallingConvention.Cdecl)]
        internal static extern NativeType ZCConfigFromStr([MarshalAs(UnmanagedType.LPStr)] string str);

        [DllImport(ZenohC.DllName, EntryPoint = "zc_config_from_file", CallingConvention = CallingConvention.Cdecl)]
        internal static extern NativeType ZCConfigFromFile([MarshalAs(UnmanagedType.LPStr)] string path);

        [DllImport(ZenohC.DllName, EntryPoint = "z_config_check", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool ZConfigCheck(ref NativeType config);

        [DllImport(ZenohC.DllName, EntryPoint = "zc_config_to_string", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr ZCConfigToString(ref NativeType config);

        [DllImport(ZenohC.DllName, EntryPoint = "zc_config_get", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr ZCConfigGet(ref NativeType config, ZString key);

        [DllImport(ZenohC.DllName, EntryPoint = "zc_config_insert_json", CallingConvention = CallingConvention.Cdecl)]
        internal static extern sbyte ZCConfigInsertJson(ref NativeType config, IntPtr key, IntPtr value);
    }
}