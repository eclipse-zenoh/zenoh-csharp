using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Zenoh
{
    public class Config
    {
        internal struct NativeType // z_owned_config_t
        {
            internal IntPtr p;
        }

        internal NativeType native;

        public Config()
        {
            this.native = ZConfigDefault();
        }


        [DllImport(Zenoh.DllName, EntryPoint = "z_config_default", CallingConvention = CallingConvention.Cdecl)]
        internal static extern NativeType ZConfigDefault();

        [DllImport(Zenoh.DllName, EntryPoint = "z_config_from_str", CallingConvention = CallingConvention.Cdecl)]
        internal static extern NativeType ZConfigFromStr([MarshalAs(UnmanagedType.LPStr)] string str);

        [DllImport(Zenoh.DllName, EntryPoint = "z_config_check", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool ZConfigCheck(ref NativeType config);

        [DllImport(Zenoh.DllName, EntryPoint = "z_config_to_str", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ZString ZConfigToStr(ref NativeType config);

        [DllImport(Zenoh.DllName, EntryPoint = "z_config_get", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ZString ZConfigGet(ref NativeType config, ZString key);

        [DllImport(Zenoh.DllName, EntryPoint = "z_config_free", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ZConfigFree(ref NativeType config);
    }
}