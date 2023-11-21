#pragma warning disable CS8500

using System;
using System.Runtime.InteropServices;

namespace Zenoh;

public class QueryOptions
{
    internal string keyexpr;
    internal QueryTarget target;
    internal ConsolidationMode mode;
    internal EncodingPrefix encodingPrefix;
    internal byte[] payload;

    public QueryOptions(string keyexpr) :
        this(keyexpr, QueryTarget.BestMatching, ConsolidationMode.Auto, EncodingPrefix.Empty, Array.Empty<byte>())
    {
    }

    public QueryOptions(string keyexpr, QueryTarget target, ConsolidationMode mode) :
        this(keyexpr, target, mode, EncodingPrefix.Empty, Array.Empty<byte>())
    {
    }

    public QueryOptions(string keyexpr, EncodingPrefix encodingPrefix, byte[] payload) :
        this(keyexpr, QueryTarget.BestMatching, ConsolidationMode.Auto, encodingPrefix, payload)
    {
    }

    public QueryOptions(string keyexpr, QueryTarget target, ConsolidationMode mode,
        EncodingPrefix encodingPrefix, byte[] payload)
    {
        this.keyexpr = keyexpr;
        this.target = target;
        this.mode = mode;
        this.encodingPrefix = encodingPrefix;
        this.payload = payload;
    }
}

public delegate void QuerierCallback(Sample sample);

public class Querier : IDisposable
{
    internal unsafe ZOwnedReplyChannel* _channel;
    internal unsafe ZOwnedReply* _reply;
    private bool _disposed;

    internal Querier()
    {
        unsafe
        {
            _disposed = false;

            ZOwnedReplyChannel channel = ZenohC.zc_reply_fifo_new(0);
            nint pChannel = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedReplyChannel>());
            Marshal.StructureToPtr(channel, pChannel, false);
            _channel = (ZOwnedReplyChannel*)pChannel;

            ZOwnedReply reply = ZenohC.z_reply_null();
            nint pReply = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedReply>());
            Marshal.StructureToPtr(reply, pReply, false);
            _reply = (ZOwnedReply*)pReply;
        }
    }

    public void Dispose() => Dispose(true);

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        unsafe
        {
            ZenohC.z_reply_drop(_reply);
            ZenohC.z_reply_channel_drop(_channel);

            Marshal.FreeHGlobal((nint)_reply);
            Marshal.FreeHGlobal((nint)_channel);
        }

        _disposed = true;
    }

    public bool GetSamples(QuerierCallback callback)
    {
        if (_disposed)
            return false;

        while (true)
        {
            unsafe
            {
                sbyte b = ZenohC.z_reply_channel_closure_call(&_channel->recv, _reply);
                if (b != 1)
                    return false;

                b = ZenohC.z_reply_check(_reply);
                if (b != 1)
                    return true;

                b = ZenohC.z_reply_is_ok(_reply);
                if (b != 1)
                    return false;

                ZSample zSample = ZenohC.z_reply_ok(_reply);
                Sample sample = new Sample(&zSample);
                callback(sample);
            }
        }
    }
}