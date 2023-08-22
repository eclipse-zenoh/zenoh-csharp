#pragma warning disable CS8500

using System.Runtime.InteropServices;

namespace Zenoh;

// z_congestion_control_t
public enum CongestionControl
{
    Block,
    Drop
}

// z_encoding_prefix_t
public enum EncodingPrefix
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
    ImageGif = 20
}

// z_sample_kind_t
public enum SampleKind
{
    Put = 0,
    Delete = 1
}

// z_priority_t
public enum Priority
{
    RealTime = 1,
    InteractiveHigh = 2,
    InteractiveLow = 3,
    DataHigh = 4,
    Data = 5,
    DataLow = 6,
    Background = 7
}

// z_consolidation_mode_t
public enum ConsolidationMode
{
    Auto = -1,
    None = 0,
    Monotonic = 1,
    Latest = 2,
}

// z_query_consolidation_t
public enum QueryConsolidation
{
    Auto = -1,
    None = 0,
    Monotonic = 1,
    Latest = 2,
}

// z_reliability_t
public enum Reliability
{
    BestEffort,
    Reliable
}

// z_query_target_t 
public enum QueryTarget
{
    BestMatching,
    All,
    AllComplete,
};

// z_bytes_t
// --------------------------------
//  typedef struct z_bytes_t {
//      size_t len;
//      const uint8_t *start;
//  } z_bytes_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ZBytes
{
    internal nuint len;
    internal byte* start;
}

// z_id_t
// --------------------------------
//  typedef struct z_id_t {
//      uint8_t id[16];
//  } z_id_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ZId
{
    // [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    internal fixed byte id[16];
}

// z_owned_str_t 
// --------------------------------
//  typedef struct z_owned_str_t {
//      char *_cstr;
//  } z_owned_str_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZOwnedStr
{
    internal nint cstr;
}

// z_owned_str_array_t
// --------------------------------
//  typedef struct z_owned_str_array_t {
//      char **val;
//      size_t len;
//  } z_owned_str_array_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZOwnedStrArray
{
    internal nint val;
    internal nuint len;
}

// z_str_array_t 
// --------------------------------
//  typedef struct z_str_array_t {
//      size_t len;
//      const char *const *val;
//  } z_str_array_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZStrArray
{
    internal nuint len;
    internal nint val;
}

// z_owned_encoding_t 
// --------------------------------
//  typedef struct z_owned_encoding_t {
//      enum z_encoding_prefix_t prefix;
//      struct z_bytes_t suffix;
//      bool _dropped;
//  } z_owned_encoding_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZOwnedEncoding
{
    internal EncodingPrefix prefix;
    internal ZBytes suffix;
    internal sbyte _dropped;
}

// z_encoding_t
// --------------------------------
//  typedef struct z_encoding_t {
//      enum z_encoding_prefix_t prefix;
//      struct z_bytes_t suffix;
//  } z_encoding_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZEncoding
{
    internal EncodingPrefix prefix;
    internal ZBytes suffix;
}

// public string PrefixToString()
// {
//     return prefix.ToString();
// }
//
// public static ZEncoding New(EncodingPrefix prefix)
// {
//     return FnZEncoding(prefix, IntPtr.Zero);
// }
//
// [DllImport(ZenohC.DllName, EntryPoint = "z_encoding", CallingConvention = CallingConvention.Cdecl)]
// internal static extern ZEncoding FnZEncoding(EncodingPrefix prefix, IntPtr suffix);

// z_timestamp_t
// --------------------------------
//  typedef struct z_timestamp_t {
//      uint64_t time;
//      struct z_id_t id;
//  } z_timestamp_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZTimestamp
{
    internal ulong time;
    internal ZId id;
}

// z_keyexpr_t  
// --------------------------------
//  typedef struct z_keyexpr_t {
//      uint64_t _0[4];
//  } z_keyexpr_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal unsafe struct ZKeyexpr
{
    // [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private fixed ulong _[4];
}

// --------------------------------
//  typedef struct z_owned_keyexpr_t {
//      uint64_t _0[4];
//  } z_keyexpr_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal unsafe struct ZOwnedKeyexpr
{
    // [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private fixed ulong _[4];
}

// z_sample_t
// --------------------------------
//  typedef struct z_sample_t {
//      struct z_keyexpr_t keyexpr;
//      struct z_bytes_t payload;
//      struct z_encoding_t encoding;
//      const void *_zc_buf;
//      enum z_sample_kind_t kind;
//      struct z_timestamp_t timestamp;
//  } z_sample_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZSample
{
    internal ZKeyexpr keyexpr;
    internal ZBytes payload;
    internal ZEncoding encoding;
    private nint _zc_buf;
    internal SampleKind kind;
    internal ZTimestamp timestamp;
}

// z_owned_config_t
// --------------------------------
//  typedef struct z_owned_config_t {
//      void *_0;
//  } z_owned_config_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZOwnedConfig
{
    private nint _;
}

// z_config_t
// --------------------------------
//  typedef struct z_config_t {
//      const struct z_owned_config_t *_0;
//  } z_config_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZConfig
{
    private nint _;
}

// z_owned_publisher_t
// --------------------------------
//  typedef struct ALIGN(8) z_owned_publisher_t {
//      uint64_t _0[7];
//  } z_owned_publisher_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal unsafe struct ZOwnedPublisher
{
    // [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
    private fixed ulong _[7];
}

// z_owned_queryable_t 
// --------------------------------
//  typedef struct ALIGN(8) z_owned_queryable_t {
//      uint64_t _0[4];
//  } z_owned_queryable_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal unsafe struct ZOwnedQueryable
{
    private fixed ulong _[4];
}

// z_publisher_t 
// --------------------------------
//  typedef struct z_publisher_t {
//      const struct z_owned_publisher_t *_0;
//  } z_publisher_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZPublisher
{
    private nint _;
}

// z_publisher_options_t 
// --------------------------------
//  typedef struct z_publisher_options_t {
//      enum z_congestion_control_t congestion_control;
//      enum z_priority_t priority;
//  } z_publisher_options_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZPublisherOptions
{
    internal CongestionControl congestion_control;
    internal Priority priority;
}

// z_publisher_delete_options_t 
// --------------------------------
//  typedef struct z_publisher_delete_options_t {
//      uint8_t __dummy;
//  } z_publisher_delete_options_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZPublisherDeleteOptions
{
    internal byte dummy;
}

// z_publisher_put_options_t 
// --------------------------------
//  typedef struct z_publisher_put_options_t {
//      struct z_encoding_t encoding;
//  } z_publisher_put_options_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZPublisherPutOptions
{
    internal ZEncoding encoding;
}

// z_pull_subscriber_options_t 
// --------------------------------
//  typedef struct z_pull_subscriber_options_t {
//      enum z_reliability_t reliability;
//  } z_pull_subscriber_options_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZPullSubscriberOptions
{
    internal Reliability reliability;
}

// z_owned_pull_subscriber_t 
// --------------------------------
//  typedef struct ALIGN(8) z_owned_pull_subscriber_t {
//      uint64_t _0[1];
//  } z_owned_pull_subscriber_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct ZOwnedPullSubscriber
{
    private ulong _;
}

// z_pull_subscriber_t 
// --------------------------------
//  typedef struct z_pull_subscriber_t {
//      const struct z_owned_pull_subscriber_t *_0;
//  } z_pull_subscriber_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZPullSubscriber
{
    internal nint _;
}

// z_queryable_options_t 
// --------------------------------
//  typedef struct z_queryable_options_t {
//      bool complete;
//  } z_queryable_options_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZQueryableOptions
{
    internal sbyte complete;
}

// z_subscriber_options_t 
// --------------------------------
// typedef struct z_subscriber_options_t {
//   enum z_reliability_t reliability;
// } z_subscriber_options_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZSubscriberOptions
{
    internal Reliability reliability;
}

// z_owned_subscriber_t 
// --------------------------------
//  typedef struct ALIGN(8) z_owned_subscriber_t {
//      uint64_t _0[1];
//  } z_owned_subscriber_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct ZOwnedSubscriber
{
    private ulong _;
}

#if PLATFORM_ARM64
// --------------------------------
//  typedef struct ALIGN(16) z_owned_reply_t {
//      uint64_t _0[24];
//  } z_owned_reply_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential, Pack = 16)]
internal unsafe struct ZOwnedReply{
    private fixed ulong _[24];
}
#elif PLATFORM_x64
// --------------------------------
//  typedef struct ALIGN(8) z_owned_reply_t {
//      uint64_t _0[22];
//  } z_owned_reply_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal unsafe struct ZOwnedReply
{
    private fixed ulong _[22];
}
#else
#error  PLATFORM_ARM64 or PLATFORM_x64
#endif

// z_delete_options_t 
// --------------------------------
//  typedef struct z_delete_options_t {
//      enum z_congestion_control_t congestion_control;
//      enum z_priority_t priority;
//  } z_delete_options_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZDeleteOptions
{
    internal CongestionControl congestion_control;
    internal Priority priority;
}

// z_query_consolidation_t 
// --------------------------------
//  typedef struct z_query_consolidation_t {
//      enum z_consolidation_mode_t mode;
//  } z_query_consolidation_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZQueryConsolidation
{
    internal ConsolidationMode mode;
}

// z_get_options_t 
// --------------------------------
//  typedef struct z_get_options_t {
//      enum z_query_target_t target;
//      struct z_query_consolidation_t consolidation;
//      struct z_value_t value;
//  } z_get_options_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZGetOptions
{
    internal QueryTarget target;
    internal ZQueryConsolidation consolidation;
    internal ZValue value;
}

// z_value_t 
// --------------------------------
//  typedef struct z_value_t {
//      struct z_bytes_t payload;
//      struct z_encoding_t encoding;
//  } z_value_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZValue
{
    internal ZBytes payload;
    internal ZEncoding encoding;
}

// zc_owned_payload_t 
// --------------------------------
//  typedef struct zc_owned_payload_t {
//      struct z_bytes_t payload;
//      uintptr_t _owner[4];
//  } zc_owned_payload_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ZcOwnedPayload
{
    private fixed ulong _[4];
}

// zc_owned_shmbuf_t 
// --------------------------------
//  typedef struct zc_owned_shmbuf_t {
//      uintptr_t _0[9];
//  } zc_owned_shmbuf_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ZcOwnedShmbuf
{
    private fixed ulong _[9];
}

// zc_owned_shm_manager_t 
// --------------------------------
//  typedef struct zc_owned_shm_manager_t {
//      uintptr_t _0;
//  } zc_owned_shm_manager_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZcOwnedShmManager
{
    private nint _;
}

// z_put_options_t 
// --------------------------------
//  typedef struct z_put_options_t {
//      struct z_encoding_t encoding;
//      enum z_congestion_control_t congestion_control;
//      enum z_priority_t priority;
//  } z_put_options_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZPutOptions
{
    internal ZEncoding encoding;
    internal CongestionControl congestionControl;
    internal Priority priority;
}

// z_query_reply_options_t 
// --------------------------------
//  typedef struct z_query_reply_options_t {
//      struct z_encoding_t encoding;
//  } z_query_reply_options_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZQueryReplyOptions
{
    internal ZEncoding encoding;
}

// z_query_t 
// --------------------------------
// typedef struct z_query_t {
//   void *_0;
// } z_query_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZQuery
{
    private nint _;
}

// z_owned_session_t 
// --------------------------------
//  typedef struct z_owned_session_t {
//    uintptr_t _0;
//  } z_owned_session_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZOwnedSession
{
    private nint _;
}

// z_session_t 
// --------------------------------
// typedef struct z_session_t {
//     uintptr_t _0;
// } z_session_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZSession
{
    private nint _;
}

// z_owned_closure_sample_t 
// --------------------------------
//  typedef struct z_owned_closure_sample_t {
//      void *context;
//      void (*call)(const struct z_sample_t*, void *context);
//      void (*drop)(void*);
//  } z_owned_closure_sample_t;
// --------------------------------
internal unsafe delegate void ZOwnedClosureSampleCall(ZSample* sample, void* context);

internal unsafe delegate void ZOwnedClosureSampleDrop(void* context);

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ZOwnedClosureSample
{
    internal void* context;
    internal ZOwnedClosureSampleCall call;
    internal ZOwnedClosureSampleDrop drop;
}

// --------------------------------
// typedef struct z_owned_closure_zid_t {
//     void *context;
//     void (*call)(const struct z_id_t*, void*);
//     void (*drop)(void*);
// } z_owned_closure_zid_t;
// --------------------------------
internal unsafe delegate void ZOwnedClosureZIdCall(ZId* zId, void* context);

internal unsafe delegate void ZOwnedClosureZIdDrop(void* context);

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ZOwnedClosureZId
{
    internal void* context;
    internal ZOwnedClosureZIdCall call;
    internal ZOwnedClosureZIdDrop drop;
}

// z_owned_closure_query_t 
// --------------------------------
//  typedef struct z_owned_closure_query_t {
//      void *context;
//      void (*call)(const struct z_query_t*, void *context);
//      void (*drop)(void*);
//  } z_owned_closure_query_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ZOwnedClosureQuery
{
    internal void* context;
    internal delegate* unmanaged[Cdecl]<ZQuery*, void*, void> call;
    internal delegate* unmanaged[Cdecl]<void*, void> drop;
}

// z_owned_closure_reply_t 
// --------------------------------
//  typedef struct z_owned_closure_reply_t {
//      void *context;
//      void (*call)(struct z_owned_reply_t*, void*);
//      void (*drop)(void*);
//  } z_owned_closure_reply_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ZOwnedClosureReply
{
    internal void* context;
    internal delegate* unmanaged[Cdecl]<ZOwnedReply*, void*, void> call;
    internal delegate* unmanaged[Cdecl]<void*, void> drop;
}

// z_owned_reply_channel_closure_t 
// --------------------------------
// typedef struct z_owned_reply_channel_closure_t {
//   void *context;
//   bool (*call)(struct z_owned_reply_t*, void*);
//   void (*drop)(void*);
// } z_owned_reply_channel_closure_t;
// --------------------------------
internal unsafe delegate void ZOwnedReplayChannelClosureCall(ZOwnedReply* zOwnedReply, void* context);

internal unsafe delegate void ZOwnedReplyChannelClosureDrop(void* context);

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ZOwnedReplyChannelClosure
{
    internal void* context;
    internal ZOwnedReplayChannelClosureCall call;
    internal ZOwnedReplyChannelClosureDrop drop;
}

// z_owned_reply_channel_t 
// --------------------------------
// typedef struct z_owned_reply_channel_t {
//     struct z_owned_closure_reply_t send;
//     struct z_owned_reply_channel_closure_t recv;
// } z_owned_reply_channel_t;
// --------------------------------
[StructLayout(LayoutKind.Sequential)]
internal struct ZOwnedReplyChannel
{
    internal ZOwnedClosureReply send;
    internal ZOwnedReplyChannelClosure recv;
}

[StructLayout(LayoutKind.Sequential)]
public struct ConsolidationStrategy // z_consolidation_strategy_t
{
    public ConsolidationMode firstRouters;
    public ConsolidationMode lastRouters;
    public ConsolidationMode reception;
}

internal static unsafe class ZenohC
{
    internal const string DllName = "zenohc";
    internal static uint zRouter = 1;
    internal static uint zPeer = 2;
    internal static uint zClient = 4;
    internal static string zConfigModeKey = "mode";
    internal static string zConfigConnectKey = "connect/endpoints";
    internal static string zConfigListenKey = "listen/endpoints";
    internal static string zConfigUserKey = "transport/auth/usrpwd/user";
    internal static string zConfigPasswordKey = "transport/auth/usrpwd/password";
    internal static string zConfigMulticastScoutingKey = "scouting/multicast/enabled";
    internal static string zConfigMulticastInterfaceKey = "scouting/multicast/interface";
    internal static string zConfigMulticastIpv4AddressKey = "scouting/multicast/address";
    internal static string zConfigScoutingTimeoutKey = "scouting/timeout";
    internal static string zConfigScoutingDelayKey = "scouting/delay";
    internal static string zConfigAddTimestampKey = "add_timestamp";

    internal static string ZOwnedStrToString(ZOwnedStr* zs)
    {
        if (z_str_check(zs) != 1)
        {
            return "";
        }

        return Marshal.PtrToStringUTF8(zs->cstr);
    }

    internal static string ZKeyexprToString(ZKeyexpr keyexpr)
    {
        ZOwnedStr str = z_keyexpr_to_string(keyexpr);
        string o = ZOwnedStrToString(&str);
        z_str_drop(&str);
        return o;
    }

    [DllImport(DllName, EntryPoint = "z_bytes_check", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_bytes_check(ZBytes* b);

    [DllImport(DllName, EntryPoint = "z_str_array_check", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_str_array_check(ZOwnedStrArray* strs);

    [DllImport(DllName, EntryPoint = "z_str_array_drop", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void z_str_array_drop(ZOwnedStrArray* strs);

    [DllImport(DllName, EntryPoint = "z_str_array_loan", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZStrArray z_str_array_loan(ZOwnedStrArray* strs);

    [DllImport(DllName, EntryPoint = "z_str_check", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_str_check(ZOwnedStr* s);

    [DllImport(DllName, EntryPoint = "z_str_drop", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void z_str_drop(ZOwnedStr* s);

    [DllImport(DllName, EntryPoint = "z_str_loan", CallingConvention = CallingConvention.Cdecl)]
    internal static extern byte* z_str_loan(ZOwnedStr* s);

    [DllImport(DllName, EntryPoint = "z_str_null", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedStr z_str_null();

    [DllImport(DllName, EntryPoint = "z_config_check", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_config_check(ZOwnedConfig* config);

    [DllImport(DllName, EntryPoint = "z_config_drop", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void z_config_drop(ZOwnedConfig* config);

    [DllImport(DllName, EntryPoint = "z_config_loan", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZConfig z_config_loan(ZOwnedConfig* config);

    [DllImport(DllName, EntryPoint = "z_config_default", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedConfig z_config_default();

    [DllImport(DllName, EntryPoint = "z_config_new", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedConfig z_config_new();

    [DllImport(DllName, EntryPoint = "z_config_null", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedConfig z_config_null();

    [DllImport(DllName, EntryPoint = "z_config_client", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedConfig z_config_client(string[] peers, nuint nPeers);

    [DllImport(DllName, EntryPoint = "z_config_peer", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedConfig z_config_peer();

    [DllImport(DllName, EntryPoint = "zc_config_from_file", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedConfig zc_config_from_file([MarshalAs(UnmanagedType.LPStr)] string path);

    [DllImport(DllName, EntryPoint = "zc_config_from_str", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedConfig zc_config_from_str([MarshalAs(UnmanagedType.LPStr)] string s);

    [DllImport(DllName, EntryPoint = "zc_config_get", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedStr zc_config_get(ZConfig config, [MarshalAs(UnmanagedType.LPStr)] string key);

    [DllImport(DllName, EntryPoint = "zc_config_insert_json", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte zc_config_insert_json(
        ZConfig config, [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string value);

    [DllImport(DllName, EntryPoint = "zc_config_to_string", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedStr zc_config_to_string(ZConfig config);

    [DllImport(DllName, EntryPoint = "z_encoding", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZEncoding z_encoding(EncodingPrefix prefix, byte* suffix);

    [DllImport(DllName, EntryPoint = "z_encoding_default", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZEncoding z_encoding_default();

    [DllImport(DllName, EntryPoint = "z_encoding_check", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_encoding_check(ZOwnedEncoding* encoding);

    [DllImport(DllName, EntryPoint = "z_encoding_drop", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void z_encoding_drop(ZOwnedEncoding* encoding);

    [DllImport(DllName, EntryPoint = "z_encoding_loan", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZEncoding z_encoding_loan(ZOwnedEncoding* encoding);

    [DllImport(DllName, EntryPoint = "z_encoding_null", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedEncoding z_encoding_null();

    [DllImport(DllName, EntryPoint = "z_keyexpr_check", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_keyexpr_check(ZOwnedKeyexpr* keyexpr);

    [DllImport(DllName, EntryPoint = "z_keyexpr", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZKeyexpr z_keyexpr(byte* name);
    // internal static extern ZKeyexpr z_keyexpr([MarshalAs(UnmanagedType.LPStr)] string name);

    [DllImport(DllName, EntryPoint = "z_keyexpr_as_bytes", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZBytes z_keyexpr_as_bytes(ZKeyexpr keyexpr);

    [DllImport(DllName, EntryPoint = "z_keyexpr_canonize", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_keyexpr_canonize(byte* start, nuint* len);

    [DllImport(DllName, EntryPoint = "z_keyexpr_canonize_null_terminated", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_keyexpr_canonize_null_terminated(byte* start);

    [DllImport(DllName, EntryPoint = "z_keyexpr_drop", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void z_keyexpr_drop(ZOwnedKeyexpr* keyexpr);

    [DllImport(DllName, EntryPoint = "z_keyexpr_concat", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedKeyexpr z_keyexpr_concat(ZKeyexpr left, byte* rightStart, nuint rightLen);

    [DllImport(DllName, EntryPoint = "z_keyexpr_equals", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_keyexpr_equals(ZKeyexpr left, ZKeyexpr right);

    [DllImport(DllName, EntryPoint = "z_keyexpr_includes", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_keyexpr_includes(ZKeyexpr left, ZKeyexpr right);

    [DllImport(DllName, EntryPoint = "z_keyexpr_intersects", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_keyexpr_intersects(ZKeyexpr left, ZKeyexpr right);

    [DllImport(DllName, EntryPoint = "z_keyexpr_is_canon", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_keyexpr_is_canon(byte* start, nuint len);

    [DllImport(DllName, EntryPoint = "z_keyexpr_is_initialized", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_keyexpr_is_initialized(ZKeyexpr* keyexpr);

    [DllImport(DllName, EntryPoint = "z_keyexpr_join", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedKeyexpr z_keyexpr_join(ZKeyexpr left, ZKeyexpr right);

    [DllImport(DllName, EntryPoint = "z_keyexpr_loan", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZKeyexpr z_keyexpr_loan(ZOwnedKeyexpr* keyexpr);

    [DllImport(DllName, EntryPoint = "z_keyexpr_new", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedKeyexpr z_keyexpr_new(byte* name);

    [DllImport(DllName, EntryPoint = "z_keyexpr_unchecked", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZKeyexpr z_keyexpr_unchecked(byte* name);

    [DllImport(DllName, EntryPoint = "z_keyexpr_null", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedKeyexpr z_keyexpr_null();

    [DllImport(DllName, EntryPoint = "z_keyexpr_to_string", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedStr z_keyexpr_to_string(ZKeyexpr keyexpr);

    [DllImport(DllName, EntryPoint = "z_declare_keyexpr", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedKeyexpr z_declare_keyexpr(ZSession session, ZKeyexpr keyexpr);

    [DllImport(DllName, EntryPoint = "z_undeclare_keyexpr", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_undeclare_keyexpr(ZSession session, ZOwnedKeyexpr* keyexpr);

    [DllImport(DllName, EntryPoint = "z_timestamp_check", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_timestamp_check(ZTimestamp ts);

    [DllImport(DllName, EntryPoint = "z_subscriber_check", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_subscriber_check(ZOwnedSubscriber* sub);

    [DllImport(DllName, EntryPoint = "z_subscriber_null", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedSubscriber z_subscriber_null();

    [DllImport(DllName, EntryPoint = "z_subscriber_options_default", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZSubscriberOptions z_subscriber_options_default();

    [DllImport(DllName, EntryPoint = "z_subscriber_pull", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_subscriber_pull(ZPullSubscriber* sub);

    [DllImport(DllName, EntryPoint = "z_declare_subscriber", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedSubscriber z_declare_subscriber(
        ZSession session, ZKeyexpr keyexpr, ZOwnedClosureSample* callback, ZSubscriberOptions* options);

    [DllImport(DllName, EntryPoint = "z_undeclare_subscriber", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_undeclare_subscriber(ZOwnedSubscriber* sub);

    [DllImport(DllName, EntryPoint = "z_pull_subscriber_check", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_pull_subscriber_check(ZOwnedPullSubscriber* sub);

    [DllImport(DllName, EntryPoint = "z_pull_subscriber_loan", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZPullSubscriber z_pull_subscriber_loan(ZOwnedPullSubscriber* sub);

    [DllImport(DllName, EntryPoint = "z_pull_subscriber_null", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedPullSubscriber z_pull_subscriber_null();

    [DllImport(DllName, EntryPoint = "z_pull_subscriber_options_default", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZPullSubscriberOptions z_pull_subscriber_options_default();

    [DllImport(DllName, EntryPoint = "z_declare_pull_subscriber", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedPullSubscriber z_declare_pull_subscriber(
        ZSession session, ZKeyexpr keyexpr, ZOwnedClosureSample* callback, ZPullSubscriberOptions* options);

    [DllImport(DllName, EntryPoint = "z_undeclare_pull_subscriber", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_undeclare_pull_subscriber(ZOwnedPullSubscriber* sub);

    [DllImport(DllName, EntryPoint = "z_declare_queryable", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedQueryable z_declare_queryable(
        ZSession session, ZKeyexpr keyexpr, ZOwnedClosureQuery* callback, ZQueryableOptions* options);

    [DllImport(DllName, EntryPoint = "z_undeclare_queryable", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_undeclare_queryable(ZOwnedQueryable* queryable);

    [DllImport(DllName, EntryPoint = "z_delete", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_delete(ZSession session, ZKeyexpr keyexpr, ZDeleteOptions* options);

    [DllImport(DllName, EntryPoint = "z_delete_options_default", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZDeleteOptions z_delete_options_default();

    [DllImport(DllName, EntryPoint = "z_get", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_get(
        ZSession session, ZKeyexpr keyexpr, byte* parameters, ZOwnedClosureReply* callback, ZGetOptions* options);

    [DllImport(DllName, EntryPoint = "z_get_options_default", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZGetOptions z_get_options_default();

    [DllImport(DllName, EntryPoint = "z_info_peers_zid", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_info_peers_zid(ZSession session, ZOwnedClosureZId* callback);

    [DllImport(DllName, EntryPoint = "z_info_routers_zid", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_info_routers_zid(ZSession session, ZOwnedClosureZId* callback);

    [DllImport(DllName, EntryPoint = "z_info_zid", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZId z_info_zid(ZSession session);

    [DllImport(DllName, EntryPoint = "z_declare_publisher", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedPublisher z_declare_publisher(
        ZSession session, ZKeyexpr keyexpr, ZPublisherOptions* options);

    [DllImport(DllName, EntryPoint = "z_undeclare_publisher", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_undeclare_publisher(ZOwnedPublisher* publisher);

    [DllImport(DllName, EntryPoint = "z_publisher_options_default", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZPublisherOptions z_publisher_options_default();

    [DllImport(DllName, EntryPoint = "z_publisher_check", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_publisher_check(ZOwnedPublisher* pbl);

    [DllImport(DllName, EntryPoint = "z_publisher_delete", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_publisher_delete(ZPublisher publisher, ZPublisherDeleteOptions* options);

    [DllImport(DllName, EntryPoint = "z_publisher_delete_options_default", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZPublisherDeleteOptions z_publisher_delete_options_default();

    [DllImport(DllName, EntryPoint = "z_publisher_loan", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZPublisher z_publisher_loan(ZOwnedPublisher* pbl);

    [DllImport(DllName, EntryPoint = "z_publisher_null", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedPublisher z_publisher_null();

    [DllImport(DllName, EntryPoint = "z_publisher_put", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_publisher_put(
        ZPublisher publisher, byte* payload, nuint len, ZPublisherPutOptions* options);

    [DllImport(DllName, EntryPoint = "zc_publisher_put_owned", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte zc_publisher_put_owned(
        ZPublisher publisher, ZcOwnedPayload* payload, ZPublisherPutOptions* options);

    [DllImport(DllName, EntryPoint = "z_publisher_put_options_default", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZPublisherOptions z_publisher_put_options_default();

    [DllImport(DllName, EntryPoint = "z_put", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_put(ZSession session, ZKeyexpr keyexpr, byte* payload, nuint len, ZPutOptions* opts);

    [DllImport(DllName, EntryPoint = "zc_put_owned", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte zc_put_owned(
        ZSession session, ZKeyexpr keyexpr, ZcOwnedPayload* payload, ZPutOptions* opts);

    [DllImport(DllName, EntryPoint = "z_put_options_default", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZPutOptions z_put_options_default();

    [DllImport(DllName, EntryPoint = "z_query_consolidation_auto", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZQueryConsolidation z_query_consolidation_auto();

    [DllImport(DllName, EntryPoint = "z_query_consolidation_default", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZQueryConsolidation z_query_consolidation_default();

    [DllImport(DllName, EntryPoint = "z_query_consolidation_latest", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZQueryConsolidation z_query_consolidation_latest();

    [DllImport(DllName, EntryPoint = "z_query_consolidation_monotonic", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZQueryConsolidation z_query_consolidation_monotonic();

    [DllImport(DllName, EntryPoint = "z_query_consolidation_none", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZQueryConsolidation z_query_consolidation_none();

    [DllImport(DllName, EntryPoint = "z_query_keyexpr", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZKeyexpr z_query_keyexpr(ZQuery* query);

    [DllImport(DllName, EntryPoint = "z_query_parameters", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZBytes z_query_parameters(ZQuery* query);

    [DllImport(DllName, EntryPoint = "z_query_reply", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_query_reply(
        ZQuery* query, ZKeyexpr key, byte* payload, nuint len, ZQueryReplyOptions* options);

    [DllImport(DllName, EntryPoint = "z_query_parameters", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZQueryReplyOptions z_query_reply_options_default();

    [DllImport(DllName, EntryPoint = "z_query_target_default", CallingConvention = CallingConvention.Cdecl)]
    internal static extern QueryTarget z_query_target_default();

    [DllImport(DllName, EntryPoint = "z_query_value", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZValue z_query_value(ZQuery* query);

    [DllImport(DllName, EntryPoint = "z_queryable_check", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_queryable_check(ZOwnedQueryable* queryable);

    [DllImport(DllName, EntryPoint = "z_queryable_null", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedQueryable z_queryable_null();

    [DllImport(DllName, EntryPoint = "z_queryable_options_default", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZQueryableOptions z_queryable_options_default();

    [DllImport(DllName, EntryPoint = "z_reply_check", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_reply_check(ZOwnedReply* replyData);

    [DllImport(DllName, EntryPoint = "z_reply_is_ok", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_reply_is_ok(ZOwnedReply* reply);

    [DllImport(DllName, EntryPoint = "z_reply_drop", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void z_reply_drop(ZOwnedReply* replyData);

    [DllImport(DllName, EntryPoint = "z_reply_err", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZValue z_reply_err(ZOwnedReply* replyData);

    [DllImport(DllName, EntryPoint = "z_reply_null", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedReply z_reply_null();

    [DllImport(DllName, EntryPoint = "z_reply_ok", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZSample z_reply_ok(ZOwnedReply* reply);

    [DllImport(DllName, EntryPoint = "z_open", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedSession z_open(ZOwnedConfig* config);

    [DllImport(DllName, EntryPoint = "z_close", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_close(ZOwnedSession* session);

    [DllImport(DllName, EntryPoint = "z_session_check", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_session_check(ZOwnedSession* session);

    [DllImport(DllName, EntryPoint = "z_session_loan", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZSession z_session_loan(ZOwnedSession* session);

    [DllImport(DllName, EntryPoint = "z_session_null", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedSession z_session_null();

    [DllImport(DllName, EntryPoint = "zc_session_rcinc", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedSession zc_session_rcinc(ZSession session);

    [DllImport(DllName, EntryPoint = "zc_init_logger", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void zc_init_logger();

    [DllImport(DllName, EntryPoint = "zc_payload_check", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte zc_payload_check(ZcOwnedPayload* payload);

    [DllImport(DllName, EntryPoint = "zc_payload_drop", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void zc_payload_drop(ZcOwnedPayload* payload);

    [DllImport(DllName, EntryPoint = "zc_payload_null", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZcOwnedPayload zc_payload_null();

    [DllImport(DllName, EntryPoint = "zc_payload_rcinc", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZcOwnedPayload zc_payload_rcinc(ZcOwnedPayload* payload);

    [DllImport(DllName, EntryPoint = "zc_sample_payload_rcinc", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZcOwnedPayload zc_sample_payload_rcinc(ZSample* sample);

    [DllImport(DllName, EntryPoint = "z_closure_sample_drop", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void z_closure_sample_drop(ZOwnedClosureSample* closure);

    [DllImport(DllName, EntryPoint = "z_closure_reply_call", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void z_closure_reply_call(ZOwnedClosureReply* closure, ZOwnedReply* sample);
    
    [DllImport(DllName, EntryPoint = "zc_reply_fifo_new", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZOwnedReplyChannel zc_reply_fifo_new(nuint bound);
    
    [DllImport(DllName, EntryPoint = "z_reply_channel_drop", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void z_reply_channel_drop(ZOwnedReplyChannel* channel);
    
    [DllImport(DllName, EntryPoint = "z_reply_channel_closure_call", CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte z_reply_channel_closure_call(ZOwnedReplyChannelClosure* closure, ZOwnedReply* reply);
}