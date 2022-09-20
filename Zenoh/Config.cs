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

            ZConfigFree(ref native);
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
            ZString zs = ZConfigToStr(ref native);
            string result = ZTypes.ZStringToString(zs);
            ZTypes.ZStringFree(ref zs);
            return result;
        }

        private bool InsertJson(string key, string value)
        {
            IntPtr k = Marshal.StringToHGlobalAnsi(key);
            IntPtr v = Marshal.StringToHGlobalAnsi(value);
            bool r = ZConfigInsertJson(ref native, k, v);
            Marshal.FreeHGlobal(k);
            Marshal.FreeHGlobal(v);
            return r;
        }

        [DllImport(Zenoh.DllName, EntryPoint = "z_config_default", CallingConvention = CallingConvention.Cdecl)]
        internal static extern NativeType ZConfigDefault();

        [DllImport(Zenoh.DllName, EntryPoint = "z_config_from_str", CallingConvention = CallingConvention.Cdecl)]
        internal static extern NativeType ZConfigFromStr([MarshalAs(UnmanagedType.LPStr)] string str);

        [DllImport(Zenoh.DllName, EntryPoint = "z_config_from_file", CallingConvention = CallingConvention.Cdecl)]
        internal static extern NativeType ZConfigFromFile([MarshalAs(UnmanagedType.LPStr)] string path);

        [DllImport(Zenoh.DllName, EntryPoint = "z_config_check", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool ZConfigCheck(ref NativeType config);

        [DllImport(Zenoh.DllName, EntryPoint = "z_config_to_str", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ZString ZConfigToStr(ref NativeType config);

        [DllImport(Zenoh.DllName, EntryPoint = "z_config_get", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ZString ZConfigGet(ref NativeType config, ZString key);

        [DllImport(Zenoh.DllName, EntryPoint = "z_config_insert_json", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool ZConfigInsertJson(ref NativeType config, IntPtr key, IntPtr value);

        [DllImport(Zenoh.DllName, EntryPoint = "z_config_free", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ZConfigFree(ref NativeType config);
    }
}