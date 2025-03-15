using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Zenoh;

public abstract class Loanable: IDisposable
{
    /// 'true' in case owned, 'false' in case loaned 
    public bool Owned { get; private protected set; }
    
    internal nint Handle{get; private protected set;}
    
    public abstract void Dispose();

    public abstract void ToOwned();

    internal abstract nint LoanedPointer();
    
    internal void CheckDisposed()
    {
        if (Handle == nint.Zero)
        {
            throw new InvalidOperationException("Object is disposed");
        }
    }
}

// z_owned_string_t
public sealed class ZString : Loanable
{
    public ZString()
    {
        var pOwnedString = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedString>());
        ZenohC.z_string_empty(pOwnedString);
        Handle = pOwnedString;
        Owned = true;
    }

    public ZString(ZString other)
    {
        other.CheckDisposed();

        var pOwnedString = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedString>());
        var pLoanedString = other.LoanedPointer();
        ZenohC.z_string_clone(pOwnedString, pLoanedString);
        Handle = pOwnedString;
        Owned = true;
    }

    public ZString(string str)
    {
        var pOwnedString = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedString>());
        var pStr = Marshal.StringToHGlobalAnsi(str);
        var r = ZenohC.z_string_copy_from_str(pOwnedString, pStr);
        if (r != Result.Ok) ZenohC.z_string_empty(pOwnedString);
        Marshal.FreeHGlobal(pStr);
        Handle = pOwnedString;
        Owned = true;
    }

    private ZString(nint handle, bool owned)
    {
        Handle = handle;
        Owned = owned;
    }

    public override void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~ZString() => Dispose(false);

    private void Dispose(bool disposing)
    {
        if (Handle == nint.Zero) return;

        if (Owned)
        {
            ZenohC.z_string_drop(Handle);
            Marshal.FreeHGlobal(Handle);
        }
        
        Handle = nint.Zero;
    }

    public override void ToOwned()
    {
        if (Owned) return;

        var pOwnedString = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedString>());
        var pLoanedString = LoanedPointer();
        ZenohC.z_string_clone(pOwnedString, pLoanedString);
        Handle = pOwnedString;
        Owned = true;
    }

    internal override nint LoanedPointer()
    {
        return Owned ? ZenohC.z_string_loan(Handle) : Handle;
    }

    internal static ZString CreateLoaned(nint handle)
    {
        return new ZString(handle, false);
    }

    internal static ZString CreateOwned()
    {
        return new ZString();
    }

    public bool IsEmpty()
    {
        CheckDisposed();

        var pLoanedString = ZenohC.z_string_loan(Handle);
        return ZenohC.z_string_is_empty(pLoanedString);
    }

    public nuint Length()
    {
        CheckDisposed();

        var pLoanedString = ZenohC.z_string_loan(Handle);
        return ZenohC.z_string_len(pLoanedString);
    }

    public override string? ToString()
    {
        CheckDisposed();

        var pLoanedString = LoanedPointer();
        var pS = ZenohC.z_string_data(pLoanedString);
        var s = Marshal.PtrToStringAnsi(pS);
        return s;
    }
}

internal sealed class ViewString : IDisposable
{
    internal nint Handle { get; private set; }

    public ViewString()
    {
        var pViewString = Marshal.AllocHGlobal(Marshal.SizeOf<ZViewString>());
        ZenohC.z_view_string_empty(pViewString);
        Handle = pViewString;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~ViewString() => Dispose(false);

    private void Dispose(bool disposing)
    {
        if (Handle == nint.Zero) return;

        Marshal.FreeHGlobal(Handle);
        Handle = nint.Zero;
    }

    internal void CheckDisposed()
    {
        if (Handle == nint.Zero)
        {
            throw new InvalidOperationException("Object has been destroyed");
        }
    }

    public override string? ToString()
    {
        CheckDisposed();

        var pLoanedString = ZenohC.z_view_string_loan(Handle);
        var pS = ZenohC.z_string_data(pLoanedString);
        return Marshal.PtrToStringAnsi(pS);
    }
}

// z_owned_bytes_t
public sealed class ZBytes :  Loanable
{
    public ZBytes()
    {
        var pOwnedBytes = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedBytes>());
        ZenohC.z_bytes_empty(pOwnedBytes);
        Handle = pOwnedBytes;
        Owned = true;
    }

    public ZBytes(ZBytes other)
    {
        other.CheckDisposed();

        var pOwnedBytes = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedBytes>());
        var pLoanedBytes = other.LoanedPointer();
        ZenohC.z_bytes_clone(pOwnedBytes,pLoanedBytes);
        Handle = pOwnedBytes;
        Owned = true;
    }

    private ZBytes(nint handle, bool owned)
    {
        Handle = handle;
        Owned = owned;
    }
    
    public override void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~ZBytes() => Dispose(false);

    private void Dispose(bool disposing)
    {
        if (Handle == nint.Zero) return;

        if (Owned)
        {
            ZenohC.z_bytes_drop(Handle);
            Marshal.FreeHGlobal(Handle);
        }

        Handle = nint.Zero;
    }

    // 'handle'  z_loaned_bytes_t*
    internal static ZBytes CreateLoaned(nint handle)
    {
        return new ZBytes(handle, false);
    }

    internal static ZBytes CreateOwned()
    {
        return new ZBytes();
    }

    public override void ToOwned()
    {
        if (Owned) return;
        
        var pOwnedBytes = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedBytes>());
        var pLoanedBytes = LoanedPointer();
        ZenohC.z_bytes_clone(pOwnedBytes,pLoanedBytes);
        Handle = pOwnedBytes;
        Owned = true;
    }

    internal override nint LoanedPointer()
    {
        return Owned ? ZenohC.z_bytes_loan(Handle) : Handle;
    }

    public bool IsEmpty()
    {
        CheckDisposed();

        var pLoanedBytes = ZenohC.z_bytes_loan(Handle);
        return ZenohC.z_bytes_is_empty(pLoanedBytes);
    }

    public nuint Length()
    {
        CheckDisposed();

        var pLoanedBytes = LoanedPointer();
        return ZenohC.z_bytes_len(pLoanedBytes);
    }

    internal nint AllocUnmanagedMemory()
    {
        var pOwnedBytes = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedBytes>());
        var pLoanedBytes = LoanedPointer();
        ZenohC.z_bytes_clone(pOwnedBytes, pLoanedBytes);
        return pOwnedBytes;
    }

    internal static void FreeUnmanagedMem(nint handle)
    {
        ZenohC.z_bytes_drop(handle);
        Marshal.FreeHGlobal(handle);
    }
}

public sealed class ZSlice : IDisposable
{
    // z_owned_slice_t*
    internal nint Handle { get; private set; }

    public ZSlice()
    {
        var pOwnedSlice = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedSlice>());
        ZenohC.z_slice_empty(pOwnedSlice);
        Handle = pOwnedSlice;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~ZSlice() => Dispose(false);

    private void Dispose(bool disposing)
    {
        if (Handle == nint.Zero) return;

        ZenohC.z_slice_drop(Handle);
        Marshal.FreeHGlobal(Handle);
        Handle = nint.Zero;
    }

    public bool IsEmpty()
    {
        if (Handle == nint.Zero)
        {
            throw new InvalidOperationException("Object has been destroyed");
        }

        var pLoanedSlice = ZenohC.z_slice_loan(Handle);
        return ZenohC.z_slice_is_empty(pLoanedSlice);
    }

    public nuint Length()
    {
        if (Handle == nint.Zero)
        {
            throw new InvalidOperationException("Object has been destroyed");
        }

        var pLoanedSlice = ZenohC.z_slice_loan(Handle);
        return ZenohC.z_slice_len(pLoanedSlice);
    }
}

public sealed class Timestamp : IDisposable
{
    // z_timestamp_t*
    internal nint Handle { get; private set; }

    private Timestamp()
    {
        throw new InvalidOperationException();
    }

    private Timestamp(nint handle)
    {
        Handle = handle;
    }

    public Timestamp(Timestamp other)
    {
        other.CheckDisposed();

        var timestamp = Marshal.PtrToStructure<ZTimestamp>(other.Handle);
        var pTimestamp = Marshal.AllocHGlobal(Marshal.SizeOf<ZTimestamp>());
        Marshal.StructureToPtr(timestamp, pTimestamp, false);
        Handle = pTimestamp;
    }

    // 'handle' z_timestamp_t*
    internal static Timestamp CloneFromPointer(nint handle)
    {
        var timestamp = Marshal.PtrToStructure<ZTimestamp>(handle);
        var pTimestamp = Marshal.AllocHGlobal(Marshal.SizeOf<ZTimestamp>());
        Marshal.StructureToPtr(timestamp, pTimestamp, false);
        return new Timestamp(pTimestamp);
    }

    // 'handle' z_loaned_session_t*
    internal static Timestamp? NewFromSession(nint handle)
    {
        var pTimestamp = Marshal.AllocHGlobal(Marshal.SizeOf<ZTimestamp>());
        var r = ZenohC.z_timestamp_new(pTimestamp, handle);
        Timestamp? o;
        if (r == Result.Ok)
        {
            o = new Timestamp(pTimestamp);
        }
        else
        {
            Marshal.FreeHGlobal(pTimestamp);
            o = null;
        }

        return o;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Timestamp() => Dispose(false);

    private void Dispose(bool disposing)
    {
        if (Handle == nint.Zero) return;

        Marshal.FreeHGlobal(Handle);
        Handle = nint.Zero;
    }

    public void CheckDisposed()
    {
        if (Handle == nint.Zero)
        {
            throw new InvalidOperationException("Object has been destroyed");
        }
    }

    public Id GetId()
    {
        CheckDisposed();

        var zid = ZenohC.z_timestamp_id(Handle);
        return new Id(zid);
    }

    public ulong Ntp64Time()
    {
        CheckDisposed();

        return ZenohC.z_timestamp_ntp64_time(Handle);
    }


    internal nint AllocUnmanagedMem()
    {
        var zTimestamp = Marshal.PtrToStructure<ZTimestamp>(Handle);
        var pTimestamp = Marshal.AllocHGlobal(Marshal.SizeOf<ZTimestamp>());
        Marshal.StructureToPtr(zTimestamp, pTimestamp, false);
        return pTimestamp;
    }

    internal static void FreeUnmanagedMem(nint handle)
    {
        Marshal.FreeHGlobal(handle);
    }
}

public sealed class Id
{
    private byte[] _data;

    private Id()
    {
        throw new InvalidOperationException();
    }

    internal Id(ZId zid)
    {
        _data = zid.GetId();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var b in _data)
        {
            sb.Append(b.ToString("x"));
        }

        return sb.ToString();
    }
}

public sealed class Keyexpr : IDisposable
{
    // z_owned_keyexpr_t*
    internal nint HandleZOwnedKeyexpr { get; private set; }

    private Keyexpr()
    {
        throw new InvalidOperationException();
    }

    private Keyexpr(nint handle)
    {
        HandleZOwnedKeyexpr = handle;
    }

    public Keyexpr(Keyexpr other)
    {
        other.CheckDisposed();

        var pOwnedKeyexpr = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedKeyexpr>());
        var pOtherKeyexpr = ZenohC.z_keyexpr_loan(other.HandleZOwnedKeyexpr);
        ZenohC.z_keyexpr_clone(pOwnedKeyexpr, pOtherKeyexpr);
        HandleZOwnedKeyexpr = pOwnedKeyexpr;
    }

    // 'handle' z_loaned_keyexpr*
    internal static Keyexpr CloneFromLoaned(nint handle)
    {
        var pOwnedKeyexpr = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedKeyexpr>());
        ZenohC.z_keyexpr_clone(pOwnedKeyexpr, handle);
        return new Keyexpr(pOwnedKeyexpr);
    }

    public static Keyexpr? Create(string keyexpr)
    {
        var pOwnedKeyexpr = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedKeyexpr>());
        var pKeyexpr = Marshal.StringToHGlobalAnsi(keyexpr);
        var r = ZenohC.z_keyexpr_from_str_autocanonize(pOwnedKeyexpr, pKeyexpr);
        Marshal.FreeHGlobal(pKeyexpr);
        if (r == Result.Ok) return new Keyexpr(pOwnedKeyexpr);

        Marshal.FreeHGlobal(pOwnedKeyexpr);
        return null;
    }


    /// Constructs key expression by performing path-joining (automatically inserting '/' in-between) of `left` with `right`.
    public static Keyexpr? Join(Keyexpr left, Keyexpr right)
    {
        var pOwnedKeyexpr = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedKeyexpr>());
        var pLeft = ZenohC.z_keyexpr_loan(left.HandleZOwnedKeyexpr);
        var pRight = ZenohC.z_keyexpr_loan(right.HandleZOwnedKeyexpr);
        var r = ZenohC.z_keyexpr_join(pOwnedKeyexpr, pLeft, pRight);
        if (r == Result.Ok) return new Keyexpr(pOwnedKeyexpr);

        Marshal.FreeHGlobal(pOwnedKeyexpr);
        return null;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Keyexpr() => Dispose(false);

    private void Dispose(bool disposing)
    {
        if (HandleZOwnedKeyexpr == IntPtr.Zero) return;

        ZenohC.z_keyexpr_drop(HandleZOwnedKeyexpr);
        Marshal.FreeHGlobal(HandleZOwnedKeyexpr);
        HandleZOwnedKeyexpr = IntPtr.Zero;
    }

    internal void CheckDisposed()
    {
        if (HandleZOwnedKeyexpr == nint.Zero)
        {
            throw new InvalidOperationException("Object has been destroyed");
        }
    }

    /// <summary>
    /// Returns ZResult.Ok if the passed string is a valid (and canon) key expression.
    /// </summary>
    /// <param name="keyexpr"></param>
    /// <returns></returns>
    public static Result IsCanon(string keyexpr)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(keyexpr);
        var len = (nuint)bytes.Length;
        Result r;
        unsafe
        {
            fixed (byte* ptr = bytes)
            {
                r = ZenohC.z_keyexpr_is_canon((nint)ptr, len);
            }
        }

        return r;
    }

    public bool Equals(Keyexpr other)
    {
        var pThis = ZenohC.z_keyexpr_loan(HandleZOwnedKeyexpr);
        var pOther = ZenohC.z_keyexpr_loan(other.HandleZOwnedKeyexpr);
        return ZenohC.z_keyexpr_equals(pThis, pOther);
    }

    /// Return true if 'this' includes 'other',
    /// i.e. the set defined by `this` contains every key belonging to the set defined by `other`.
    public bool Includes(Keyexpr other)
    {
        var pThis = ZenohC.z_keyexpr_loan(HandleZOwnedKeyexpr);
        var pOther = ZenohC.z_keyexpr_loan(other.HandleZOwnedKeyexpr);
        return ZenohC.z_keyexpr_includes(pThis, pOther);
    }

    /// Returns true if the keyexprs intersect,
    /// i.e. there exists at least one key which is contained in both of the sets defined by `this` and `other`.
    public bool Intersects(Keyexpr other)
    {
        var pThis = ZenohC.z_keyexpr_loan(HandleZOwnedKeyexpr);
        var pOther = ZenohC.z_keyexpr_loan(other.HandleZOwnedKeyexpr);
        return ZenohC.z_keyexpr_intersects(pThis, pOther);
    }

    public override string? ToString()
    {
        var viewString = new ViewString();
        var pLoanedKeyexpr = ZenohC.z_keyexpr_loan(HandleZOwnedKeyexpr);
        ZenohC.z_keyexpr_as_view_string(pLoanedKeyexpr, viewString.Handle);
        var s = viewString.ToString();
        viewString.Dispose();
        return s;
    }
}

// z_owned_sample_t
public sealed class Sample : Loanable
{
    private Sample()
    {
        throw new InvalidOperationException();
    }

    internal Sample(Sample other)
    {
        other.CheckDisposed();
        
        var pOwnedSample = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedSample>());
        var pLoanedSample = other.LoanedPointer();
        ZenohC.z_sample_clone(pOwnedSample, pLoanedSample);
        Owned = true;
        Handle = pOwnedSample;
    }

    private Sample(nint handle, bool owned)
    {
        Owned = owned;
        Handle = handle;
    }

    internal static Sample CreateLoaned(nint handle)
    {
        return new Sample(handle, false);
    }

    internal static Sample CreateOwned()
    {
        var pOwnedSample = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedSample>());
        ZenohC.z_internal_sample_null(pOwnedSample);
        return new Sample(pOwnedSample, true);
    }

    ~Sample()
    {
        Dispose(false);
    }

    public override void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (Handle == nint.Zero) return;

        if (Owned)
        {
            ZenohC.z_sample_drop(Handle);
            Marshal.FreeHGlobal(Handle);
        }

        Handle = nint.Zero;
    }

    public override void ToOwned()
    {
        if (Owned) return;
        
        var pOwnedSample = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedSample>());
        var pLoanedSample = LoanedPointer();
        ZenohC.z_sample_clone(pOwnedSample, pLoanedSample);
        Owned = true;
        Handle = pOwnedSample;
    }

    internal override nint LoanedPointer()
    {
        return Owned? ZenohC.z_sample_loan(Handle) : Handle;
    }

    /// <summary>
    /// Returns sample attachment.
    /// </summary>
    /// <returns>
    /// Returns 'null', if sample does not contain any attachment.
    /// </returns>
    public ZBytes? GetAttachment()
    {
        CheckDisposed();

        var pLoanedSample = Owned ? ZenohC.z_sample_loan(Handle) : Handle;
        var pLoanedBytes = ZenohC.z_sample_attachment(pLoanedSample);
        return pLoanedBytes == nint.Zero ? null : ZBytes.CreateLoaned(pLoanedBytes);
    }

    /// <summary>
    /// Returns sample qos congestion control value.
    /// </summary>
    /// <returns></returns>
    public CongestionControl GetCongestionControl()
    {
        CheckDisposed();

        var pLoanedSample = Owned ? ZenohC.z_sample_loan(Handle) : Handle;
        return ZenohC.z_sample_congestion_control(pLoanedSample);
    }

    /// <summary>
    /// Returns the encoding associated with the sample data.
    /// </summary>
    /// <returns></returns>
    public Encoding GetEncoding()
    {
        CheckDisposed();

        var pLoanedSample = Owned ? ZenohC.z_sample_loan(Handle) : Handle;
        var pLoanedEncoding = ZenohC.z_sample_encoding(pLoanedSample);
        return Encoding.CreateLoaned(pLoanedEncoding);
    }

    /// <summary>
    /// <para>Gets the express flag value.</para>
    /// <para>
    /// If true, the message is not batched during transmission, in order to reduce latency.
    /// </para>
    /// </summary>
    public bool GetExpress()
    {
        CheckDisposed();

        var pLoanedSample = Owned ? ZenohC.z_sample_loan(Handle) : Handle;
        return ZenohC.z_sample_express(pLoanedSample);
    }

    /// <summary>
    /// Returns the key expression of the sample.
    /// </summary>
    public Keyexpr GetKeyexpr()
    {
        CheckDisposed();

        var pLoanedSample = Owned ? ZenohC.z_sample_loan(Handle) : Handle;
        var pLoanedKeyexpr = ZenohC.z_sample_keyexpr(pLoanedSample);
        return Keyexpr.CloneFromLoaned(pLoanedKeyexpr);
    }

    /// <summary>
    /// Returns the sample kind.
    /// </summary>
    /// <returns></returns>
    public SampleKind GetKind()
    {
        CheckDisposed();

        var pLoanedSample = Owned ? ZenohC.z_sample_loan(Handle) : Handle;
        return ZenohC.z_sample_kind(pLoanedSample);
    }

    /// <summary>
    /// Returns the sample payload data.
    /// </summary>
    /// <returns></returns>
    public ZBytes GetPayload()
    {
        CheckDisposed();

        var pLoanedSample = Owned ? ZenohC.z_sample_loan(Handle) : Handle;
        var pLoanedBytes = ZenohC.z_sample_payload(pLoanedSample);
        return ZBytes.CreateLoaned(pLoanedBytes);
    }

    /// <summary>
    /// Returns sample qos priority value.
    /// </summary>
    /// <returns></returns>
    public Priority GetPriority()
    {
        CheckDisposed();

        var pLoanedSample = Owned ? ZenohC.z_sample_loan(Handle) : Handle;
        return ZenohC.z_sample_priority(pLoanedSample);
    }

    /// <summary>
    /// Returns the sample timestamp.
    /// </summary>
    /// <returns>
    /// Will return 'null', if sample is not associated with a timestamp.
    /// </returns>
    public Timestamp? GetTimestamp()
    {
        CheckDisposed();

        var pLoanedSample = Owned ? ZenohC.z_sample_loan(Handle) : Handle;
        var pTimestamp = ZenohC.z_sample_timestamp(pLoanedSample);
        return pTimestamp == nint.Zero ? null : Timestamp.CloneFromPointer(pTimestamp);
    }
}

public sealed class ReplyErr : IDisposable
{
    // 'true' in case z_owned_reply_err_t, 'false' in case z_loaned_reply_err_t 
    internal bool Owned { get; }

    // z_loaned_reply_err_t*  or  z_owned_reply_err_t*
    internal nint Handle { get; private set; }

    private ReplyErr()
    {
        throw new InvalidOperationException();
    }

    private ReplyErr(nint handle, bool owned)
    {
        Owned = owned;
        Handle = handle;
    }

    internal static ReplyErr CreateLoaned(nint handle)
    {
        return new ReplyErr( handle, false);
    }

    internal static ReplyErr CreateOwned()
    {
        var pOwnedReplyErr = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedReplyErr>());
        ZenohC.z_internal_reply_err_null(pOwnedReplyErr);
        return new ReplyErr(pOwnedReplyErr,true);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    ~ReplyErr()=>Dispose(false);

    private void Dispose(bool disposing)
    {
        if (Handle == nint.Zero) return;

        if (Owned)
        {
            ZenohC.z_reply_err_drop(Handle);
            Marshal.FreeHGlobal(Handle);
        }
        
        Handle = nint.Zero;
    }
}

public sealed class Reply : IDisposable
{
    // z_owned_reply_t*
    internal nint Handle { get;  private set; }

    internal Reply()
    {
        var pOwnedReply = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedReply>());
        ZenohC.z_internal_reply_null(pOwnedReply);
        Handle = pOwnedReply;
    }

    public Reply(Reply other)
    {
        other.CheckDisposed();
        
        var pOwnedReply = Marshal.AllocHGlobal(Marshal.SizeOf<ZOwnedReply>());
        var pLoanedReply = ZenohC.z_reply_loan(other.Handle);
        ZenohC.z_reply_clone(pOwnedReply, pLoanedReply);
        Handle = pOwnedReply;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    ~Reply()=>Dispose(false);

    private void Dispose(bool disposing)
    {
        if (Handle == nint.Zero) return;
        
        ZenohC.z_reply_drop(Handle);
        Marshal.FreeHGlobal(Handle);
        Handle = nint.Zero;
    }
    
    internal void CheckDisposed()
    {
        if (Handle == nint.Zero)
        {
            throw new InvalidOperationException("Object is disposed");
        }
    }

    /// <summary>
    /// Returns 'true' if reply contains a valid response, 'false' otherwise.
    /// </summary>
    public bool IsOk()
    {
        CheckDisposed();
        
        var pLoanedReply = ZenohC.z_reply_loan(Handle);
        return ZenohC.z_reply_is_ok(pLoanedReply);
    }
    
}