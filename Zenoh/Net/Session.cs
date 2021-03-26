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

namespace Zenoh.Net
{

    public class Session : IDisposable
    {
        private IntPtr /*zn_session_t*/ _nativePtr = IntPtr.Zero;

        private Session(IntPtr /*zn_session_t*/ nativeSession)
        {
            this._nativePtr = nativeSession;
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (this._nativePtr != IntPtr.Zero)
            {
                ZnClose(this._nativePtr);
                this._nativePtr = IntPtr.Zero;
            }
        }

        public static Session Open(Dictionary<string, string> config)
        {
            // It's simpler to encode the config as a single string to pass it to Rust where is will be decoded
            string configStr = "";
            foreach (KeyValuePair<string, string> kvp in config)
            {
                configStr += kvp.Key + "=" + kvp.Value + ";";
            }
            var props = Zenoh.ZnConfigFromStr(configStr);

            var nativeSession = ZnOpen(props);
            // TODO: check errors...
            return new Session(nativeSession);
        }

        public Dictionary<string, string> Info()
        {
            var zstr = ZnInfoAsStr(this._nativePtr);
            return ZTypes.ZStringToProperties(zstr);
        }

        public ulong DeclareResource(ResKey reskey)
        {
            return ZnDeclareResource(this._nativePtr, reskey._key);
        }

        unsafe public void Write(ResKey reskey, byte[] payload)
        {
            fixed (byte* p = payload)
            {
                ZnWrite(this._nativePtr, reskey._key, (IntPtr)p, (uint)payload.Length);
            }
        }

        unsafe public void Write(ResKey reskey, byte[] payload, uint encoding, uint kind, CongestionControl congestionControl)
        {
            fixed (byte* p = payload)
            {
                ZnWriteExt(this._nativePtr, reskey._key, (IntPtr)p, (uint)payload.Length, encoding, kind, congestionControl);
            }
        }

        // Implementation of the zenoh-c callback that will call the C# user's callback passed as IntPtr
        private static SubscriberCallbackNative TheSubscriberCallbackNative = new SubscriberCallbackNative(SubscriberCallbackNativeImpl);

        internal static void SubscriberCallbackNativeImpl(IntPtr /* *const zn_sample_t */ samplePtr, IntPtr /* *const c_void */ callBackPtr)
        {
            Sample s = new Sample(samplePtr);
            try
            {
                SubscriberCallback callback = Marshal.GetDelegateForFunctionPointer<SubscriberCallback>(callBackPtr);
                callback(s);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Exception in SubscriberCallback triggered by Sample on {0}: {1}", s.ResName, e);
                Console.WriteLine(e.StackTrace);
            }
        }

        public Subscriber DeclareSubscriber(ResKey reskey, SubInfo subInfo, SubscriberCallback callback)
        {
            // convert the C# user callback into an IntPtr to be passed as "arg" to zn_declare_subscriber()
            // and received back in each call to SubscriberCallbackNativeImpl()
            IntPtr callbackPtr = Marshal.GetFunctionPointerForDelegate(callback);
            SubscriberCallbackNative callbackNative = new SubscriberCallbackNative(SubscriberCallbackNativeImpl);
            var nativeSubscriber = ZnDeclareSubscriber(this._nativePtr, reskey._key, subInfo._subInfo, callbackNative, callbackPtr);
            return new Subscriber(nativeSubscriber, callbackNative, callback);
        }

        [DllImport("zenohc", EntryPoint = "zn_open")]
        internal static extern IntPtr /*zn_session_t*/ ZnOpen(IntPtr /*zn_properties_t*/ config);

        [DllImport("zenohc", EntryPoint = "zn_info_as_str")]
        internal static extern ZString ZnInfoAsStr(IntPtr /*zn_session_t*/ rustSession);

        [DllImport("zenohc", EntryPoint = "zn_close")]
        internal static extern void ZnClose(IntPtr /*zn_session_t*/ rustSession);

        [DllImport("zenohc", EntryPoint = "zn_declare_resource")]
        internal static extern ulong ZnDeclareResource(IntPtr /*zn_session_t*/ rustSession, ResKey.NativeType zResKey);

        [DllImport("zenohc", EntryPoint = "zn_write")]
        internal static extern int ZnWrite(
            IntPtr /*zn_session_t*/ rustSession,
            ResKey.NativeType zResKey,
            IntPtr payload,
            uint len);

        [DllImport("zenohc", EntryPoint = "zn_write_ext")]
        internal static extern int ZnWriteExt(
            IntPtr /*zn_session_t*/ rustSession,
            ResKey.NativeType zResKey,
            IntPtr payload,
            uint len,
            uint encoding,
            uint kind,
            CongestionControl congestion);

        [DllImport("zenohc", EntryPoint = "zn_declare_subscriber")]
        internal static extern IntPtr /*zn_subscriber_t*/ ZnDeclareSubscriber(
            IntPtr /*zn_session_t*/ rustSession,
            ResKey.NativeType zResKey,
            SubInfo.NativeType zSubInfo,
            SubscriberCallbackNative callback,
            IntPtr /*void* */ arg);

    }
}