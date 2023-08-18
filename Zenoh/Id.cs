using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Zenoh;

public struct Id
{
    internal byte[] data;

    internal Id(ZId zid)
    {
        data = new byte[16];
        for (int i = 0; i < 16; i++)
        {
            unsafe
            {
                data[i] = zid.id[i];
            }
        }
    }

    public string ToStr()
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in data)
        {
            sb.Append(b.ToString("x"));
        }

        return sb.ToString();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal unsafe struct ZIdBuffer
{
    internal nuint count = 0;
    internal fixed byte data[256 * 16];

    public ZIdBuffer()
    {
    }

    internal void Add(ZId* zId)
    {
        if (count >= 256) return;
        for (nuint i = 0; i < 16; i++)
        {
            data[count * 16 + i] = zId->id[i];
        }

        count += 1;
    }

    internal Id[] ToIds()
    {
        Id[] ids = new Id[count];
        for (nuint i = 0; i < count; i++)
        {
            byte[] o = new byte[16];
            for (nuint j = 0; j < 16; j++)
            {
                o[j] = data[i * 16 + j];
            }

            Id id = new Id
            {
                data = o,
            };
            ids[i] = id;
        }

        return ids;
    }

    internal static void z_id_call(ZId* zId, void* context)
    {
        ZIdBuffer* pIdBuffer = (ZIdBuffer*)context;
        pIdBuffer->Add(zId);
    }
}