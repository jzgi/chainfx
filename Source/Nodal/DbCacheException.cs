using System;

namespace ChainFX.Nodal;

/// <summary>
/// Thrown to indicate an error related to database to in-memory cache.
/// </summary>
public class DbCacheException : Exception
{
    /// <summary>
    /// The returned error code.
    /// </summary>
    public int Code { get; internal set; }

    public DbCacheException()
    {
    }

    public DbCacheException(string msg) : base(msg)
    {
    }

    public DbCacheException(string msg, Exception inner) : base(msg, inner)
    {
    }
}