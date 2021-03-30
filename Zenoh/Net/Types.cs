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


namespace Zenoh.Net
{
    public class ResKey
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct NativeType    // zn_reskey_t
        {
            internal ulong id;        // c_ulong
            internal IntPtr suffix;   // *const c_char
        }

        internal NativeType _key;

        public ulong Id
        {
            get
            {
                return _key.id;
            }
        }

        public string Suffix
        {
            get
            {
                return Marshal.PtrToStringAnsi(_key.suffix);
            }
        }

        internal ResKey(NativeType resKey)
        {
            this._key = resKey;
        }

        public static ResKey RId(ulong id)
        {
            NativeType resKey;
            resKey.id = id;
            resKey.suffix = IntPtr.Zero;
            return new ResKey(resKey);
        }

        public static ResKey RName(string suffix)
        {
            NativeType resKey;
            resKey.id = 0;
            resKey.suffix = Marshal.StringToHGlobalAnsi(suffix);
            return new ResKey(resKey);
        }

        public static ResKey RIdWithSuffix(ulong id, string suffix)
        {
            NativeType resKey;
            resKey.id = id;
            resKey.suffix = Marshal.StringToHGlobalAnsi(suffix);
            return new ResKey(resKey);
        }

        public bool IsNumerical()
        {
            return _key.suffix == IntPtr.Zero;
        }
    }

    public enum CongestionControl
    {
        BLOCK,
        DROP
    }

    public class Sample
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct NativeType   // zn_sample_t
        {
            internal ZString key;    // z_string_t
            internal ZBytes value;   // z_bytes_t
        }

        public string ResName
        { get; }
        public byte[] Payload
        { get; }

        unsafe internal Sample(IntPtr /* *const zn_sample_t */ _sample)
        {
            // Note: copies are made here. Could we avoid this ?
            NativeType* s = (NativeType*)_sample;
            ResName = ZTypes.ZStringToString(s->key);
            Payload = ZTypes.ZBytesToBytesArray(s->value);
        }

        internal Sample(NativeType _sample)
        {
            // Note: copies are made here. Could we avoid this ?
            ResName = ZTypes.ZStringToString(_sample.key);
            Payload = ZTypes.ZBytesToBytesArray(_sample.value);
            ZnSampleFree(_sample);
        }

        [DllImport("zenohc", EntryPoint = "zn_sample_free")]
        internal static extern void ZnSampleFree(NativeType sample);
    }

    public enum Reliability
    {
        BEST_EFFORT,
        RELIABLE,
    }

    public enum SubMode
    {
        PUSH,
        PULL,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Period   // zn_period_t
    {
        public uint origin;    // c_uint
        public uint period;    // c_uint
        public uint duration;  // c_uint

        public Period(uint origin, uint period, uint duration)
        {
            this.origin = origin;
            this.period = period;
            this.duration = duration;
        }

        public static Period None()
        {
            return new Period(0, 0, 0);
        }

        public override string ToString()
        {
            return String.Format("origin={0}, period={1}, duration={2}", origin, period, duration);
        }
    }


    public class SubInfo
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeType              // zn_subinfo_t
        {
            public Reliability reliability;   // zn_reliability_t
            public SubMode mode;              // zn_submode_t
            public IntPtr period;             // *mut zn_period_t
        }

        public NativeType _subInfo;

        public SubInfo()
        {
            this._subInfo = ZnSubInfoDefault();
        }

        public SubInfo(SubMode mode) : this()
        {
            _subInfo.mode = mode;
        }

        // TODO: add period param
        public SubInfo(Reliability reliability, SubMode mode) : this()
        {
            _subInfo.reliability = reliability;
            _subInfo.mode = mode;
        }

        [DllImport("zenohc", EntryPoint = "zn_subinfo_default")]
        internal static extern NativeType ZnSubInfoDefault();
    }

    // SubscriberCallback to be implemented by user.
    public delegate void SubscriberCallback(Sample sample);

    // Type of the callback function expected by zenoh-c
    internal delegate void SubscriberCallbackNative(IntPtr /* *const zn_sample_t */ samplePtr, IntPtr /* *const c_void */ callBackPtr);

    public class Subscriber
    {
        private Session _session;
        private Int32 _subscriberHandle;
        private IntPtr /*zn_subscriber_t*/ _nativePtr = IntPtr.Zero;
        internal SubscriberCallback UserCallback;

        internal Subscriber(Session session, Int32 subscriberHandle, IntPtr nativeSubscriber, SubscriberCallback userCallback)
        {
            this._session = session;
            this._subscriberHandle = subscriberHandle;
            this._nativePtr = nativeSubscriber;
            this.UserCallback = userCallback;
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_nativePtr != IntPtr.Zero)
            {
                Session.ZnUndeclareSubscriber(_nativePtr);
                _session.Subscribers.Remove(_subscriberHandle);
                _nativePtr = IntPtr.Zero;
            }
        }
    }
}