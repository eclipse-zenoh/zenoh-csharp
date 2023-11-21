#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Zenoh;

public class Config : IDisposable
{
    public enum Mode
    {
        Peer,
        Client,
    }

    internal unsafe ZOwnedConfig* ownedConfig;
    private bool _disposed;


    public Config()
    {
        unsafe
        {
            ZOwnedConfig config = ZenohC.z_config_default();
            nint p = Marshal.AllocHGlobal(Marshal.SizeOf(config));
            Marshal.StructureToPtr(config, p, false);
            ownedConfig = (ZOwnedConfig*)p;
        }
    
        _disposed = false;
    }

    public static Config? LoadFromFile(string path)
    {
        unsafe
        {
            ZOwnedConfig config = ZenohC.zc_config_from_file(path);
            int b = ZenohC.z_config_check(&config);
            // if (!ZenohC.z_config_check(&config))
            if (b==0)
            {
                ZenohC.z_config_drop(&config);
                return null;
            }

            nint p = Marshal.AllocHGlobal(Marshal.SizeOf(config));
            Marshal.StructureToPtr(config, p, false);
            return new Config
            {
                ownedConfig = (ZOwnedConfig*)p,
                _disposed = false,
            };
        }
    }

    public void Dispose() => Dispose(true);

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        unsafe
        {
            ZenohC.z_config_drop(ownedConfig);
            Marshal.FreeHGlobal((nint)ownedConfig);
        }

        _disposed = true;
    }

    public string ToStr()
    {
        if (_disposed) return "";

        unsafe
        {
            ZConfig config = ZenohC.z_config_loan(ownedConfig);
            ZOwnedStr str = ZenohC.zc_config_to_string(config);
            string o = ZenohC.ZOwnedStrToString(&str);
            ZenohC.z_str_drop(&str);
            return o;
        }
    }


    public bool SetMode(Mode mode)
    {
        if (_disposed) return false;

        string value = "";
        switch (mode)
        {
            case Mode.Client:
                value = "\"client\"";
                break;
            case Mode.Peer:
                value = "\"peer\"";
                break;
        }

        unsafe
        {
            ZConfig config = ZenohC.z_config_loan(ownedConfig);
            sbyte r = ZenohC.zc_config_insert_json(config, ZenohC.zConfigModeKey, value);
            return r == 0;
        }
    }

    // The v such as "tcp/172.30.1.1:7447"
    public bool SetConnect(string[] v)
    {
        if (_disposed) return false;

        StringBuilder value = new StringBuilder("[");
        foreach (var ele in v)
        {
            value.Append($"\"{ele}\",");
        }

        value.Append("]");

        unsafe
        {
            ZConfig config = ZenohC.z_config_loan(ownedConfig);
            sbyte r = ZenohC.zc_config_insert_json(config, ZenohC.zConfigConnectKey, value.ToString());
            return r == 0;
        }
    }

    // The v such as "tcp/127.0.0.1:7888"
    public bool SetListen(string[] v)
    {
        if (_disposed) return false;

        StringBuilder value = new StringBuilder("[");
        foreach (var ele in v)
        {
            value.Append($"\"{ele}\",");
        }

        value.Append("]");

        unsafe
        {
            ZConfig config = ZenohC.z_config_loan(ownedConfig);
            sbyte r = ZenohC.zc_config_insert_json(config, ZenohC.zConfigListenKey, value.ToString());
            return r == 0;
        }
    }

    // Whether data messages should be timestamped
    public bool SetTimestamp(bool b)
    {
        if (_disposed) return false;

        string value = b ? "true" : "false";

        unsafe
        {
            ZConfig config = ZenohC.z_config_loan(ownedConfig);
            sbyte r = ZenohC.zc_config_insert_json(config, ZenohC.zConfigAddTimestampKey, value);
            return r == 0;
        }
    }
}