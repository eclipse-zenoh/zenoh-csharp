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
using System.Linq;


namespace Zenoh
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ZString
    {
        public IntPtr val;  // *const c_char
        public IntPtr len;  // size_t
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ZBytes
    {
        public IntPtr val;  // *const u8
        public IntPtr len;  // size_t
    }

    internal static class ZTypes
    {
        internal static string ZStringToString(ZString zs)
        {
            byte[] managedArray = new byte[(int)zs.len];
            System.Runtime.InteropServices.Marshal.Copy(zs.val, managedArray, 0, (int)zs.len);
            string result = System.Text.Encoding.UTF8.GetString(managedArray, 0, (int)zs.len);
            // TODO Free ZString ???
            return result;
        }

        internal static byte[] ZBytesToBytesArray(ZBytes zb)
        {
            byte[] managedArray = new byte[(int)zb.len];
            System.Runtime.InteropServices.Marshal.Copy(zb.val, managedArray, 0, (int)zb.len);
            // TODO Free ZBytes ???
            return managedArray;
        }

        private static char[] _propSeparator = { ';' };
        private static char[] _kvSeparator = { '=' };

        internal static Dictionary<string, string> ZStringToProperties(ZString zs)
        {
            var str = ZTypes.ZStringToString(zs);

            // Parse the properties from the string
            var properties = str.Split(_propSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(_kvSeparator, 2))
                .ToDictionary(x => x.First(), x => (x.Length == 2) ? x.Last() : "");
            return properties;
        }

    }

}