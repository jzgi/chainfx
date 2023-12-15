using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChainFX.Nodal;

/// <summary>
/// A cache for a single map polulated from database. 
/// </summary>
/// <typeparam name="K">the type of the keys in the cache</typeparam>
/// <typeparam name="V">the type of the values in the cache</typeparam>
internal class DbMap<K, V> : DbCache where K : IComparable<K>, IEquatable<K>
{
    private readonly bool async;

    // either readerwriterlock or semaphore
    protected readonly IDisposable slim;

    protected Map<K, V> data;

    // tick count,   
    private volatile int expiry;


    internal DbMap(Func<DbContext, Map<K, V>> fetch, Type typ, int maxage, byte flag) :
        base(fetch, typ, maxage, flag)
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
    internal DbMap(Func<DbContext, Task<Map<K, V>>> fetch, Type typ, int maxage, byte flag) :
        base(fetch, typ, maxage, flag)
    {
        async = true;
        slim = new SemaphoreSlim(1, 1);
    }

    public override bool IsAsync => async;


    public Map<K, V> Get()
    {
        if (!(fetch is Func<DbContext, Map<K, V>> func)) // check fetcher
        {
            throw new DbCacheException("incorrect fetcher for " + Typ);
        }

        var lck = (ReaderWriterLockSlim)slim;
        lck.EnterUpgradeableReadLock();
        try
        {
            var tick = Environment.TickCount & int.MaxValue; // positive tick
            if (tick >= expiry) // if expires
            {
                lck.EnterWriteLock();
                try
                {
                    // re-fetch
                    using var dc = Nodality.NewDbContext();
                    data = func(dc);
                    expiry = tick + MaxAge * 1000;
                }
                finally
                {
                    lck.ExitWriteLock();
                }
            }
            return data;
        }
        finally
        {
            lck.ExitUpgradeableReadLock();
        }
    }

    public async Task<Map<K, V>> GetAsync()
    {
        if (!(fetch is Func<DbContext, Task<Map<K, V>>> func)) // check fetcher
        {
            throw new DbCacheException("incorrect fetcher for " + Typ);
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
                data = await func(dc);
                expiry = tick + MaxAge * 1000;
            }

            return data;
        }
        finally
        {
            semaphore.Release();
        }
    }
}