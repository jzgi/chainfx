using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChainFx.Nodal;

/// <summary>
/// A cache for single entire map.
/// </summary>
internal class DbUniCache<K, V> : DbCache where K : IComparable<K>, IEquatable<K>
{
    readonly bool async;

    // either readerwriterlock or semaphore
    protected readonly IDisposable slim;

    protected Map<K, V> cached;

    // tick count,   
    volatile int expiry;


    internal DbUniCache(Func<DbContext, Map<K, V>> fetch, Type typ, int maxage, byte flag) : base(fetch, typ, maxage, flag)
    {
        async = false;
        slim = new ReaderWriterLockSlim();
    }

    /// <summary>
    /// The async version of constructor.
    /// </summary>
    /// <param name="fetch"></param>
    /// <param name="typ"></param>
    /// <param name="maxage"></param>
    /// <param name="flag"></param>
    internal DbUniCache(Func<DbContext, Task<Map<K, V>>> fetch, Type typ, int maxage, byte flag) : base(fetch, typ, maxage, flag)
    {
        async = true;
        slim = new SemaphoreSlim(1, 1);
    }

    public override bool IsAsync => async;


    public Map<K, V> Get()
    {
        if (!(fetch is Func<DbContext, Map<K, V>> func)) // check fetcher
        {
            throw new DbException("Missing fetcher for " + Typ);
        }
        var @lock = (ReaderWriterLockSlim)slim;
        @lock.EnterUpgradeableReadLock();
        try
        {
            var tick = Environment.TickCount & int.MaxValue; // positive tick
            if (tick >= expiry) // if expires
            {
                @lock.EnterWriteLock();
                try
                {
                    // re-fetch
                    using var dc = Nodality.NewDbContext();
                    cached = func(dc);
                    expiry = tick + MaxAge * 1000;
                }
                finally
                {
                    @lock.ExitWriteLock();
                }
            }
            return cached;
        }
        finally
        {
            @lock.ExitUpgradeableReadLock();
        }
    }

    public async Task<Map<K, V>> GetAsync()
    {
        if (!(fetch is Func<DbContext, Task<Map<K, V>>> func)) // check fetcher
        {
            throw new DbException("Wrong fetcher for " + Typ);
        }
        var semaphore = (SemaphoreSlim)slim;
        await semaphore.WaitAsync();
        try
        {
            var tick = Environment.TickCount & int.MaxValue; // positive tick
            if (tick >= expiry)
            {
                // re-fetch
                using var dc = Nodality.NewDbContext();
                cached = await func(dc);
                expiry = tick + MaxAge * 1000;
            }

            return cached;
        }
        finally
        {
            semaphore.Release();
        }
    }
}