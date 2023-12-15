using System;

namespace ChainFX.Nodal;

/// <summary>
/// Thrown to indicate an error during database operation.
/// </summary>
public class DbException : Exception
{
    /// <summary>
    /// The returned error code.
    /// </summary>
    public int Code { get; internal set; }

    public DbException()
    {
    }

    public DbException(string msg) : base(msg)
    {
    }

    public DbException(string msg, Exception inner) : base(msg, inner)
    {
    }
}