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
    public class Zenoh
    {

        [DllImport("zenohc", EntryPoint = "z_init_logger")]
        public static extern void InitLogger();

        private static char[] _propSeparator = { ';' };
        private static char[] _kvSeparator = { '=' };

        public static Dictionary<string, string> ConfigFromFile(string path)
        {
            var zprops = ZnConfigFromFile(path);
            var zstr = ZnConfigToStr(zprops);
            return ZTypes.ZStringToProperties(zstr);
        }

        [DllImport("zenohc", EntryPoint = "zn_config_from_str", CharSet = CharSet.Ansi)]
        internal static extern IntPtr /*zn_properties_t*/ ZnConfigFromStr([MarshalAs(UnmanagedType.LPStr)] string str);

        [DllImport("zenohc", EntryPoint = "zn_config_to_str", CharSet = CharSet.Ansi)]
        internal static extern ZString ZnConfigToStr(IntPtr /*zn_properties_t*/ znProps);

        [DllImport("zenohc", EntryPoint = "zn_config_from_file", CharSet = CharSet.Ansi)]
        internal static extern IntPtr /*zn_properties_t*/ ZnConfigFromFile([MarshalAs(UnmanagedType.LPStr)] string str);

    }

}