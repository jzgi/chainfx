using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CoChain.Nodal
{
    internal abstract class DbCache
    {
        // the actual type cached (and to seek for)
        readonly Type typ;

        // bitwise matcher
        readonly short flag;

        // in seconds
        readonly int maxage;

        // either of the two forms
        protected readonly Delegate fetcher;

        // tick count,   
        protected int expiry;


        protected DbCache(Delegate fetcher, Type typ, int maxage, short flag)
        {
            this.fetcher = fetcher;
            this.typ = typ;
            this.maxage = maxage;
            this.flag = flag;
        }


        public Delegate Fetcher => fetcher;

        public abstract bool IsAsync { get; }

        public Type Typ => typ;

        public int MaxAge => maxage;

        public short Flag => flag;
    }


    /// <summary>
    /// A cache for single whole map.
    /// </summary>
    internal class DbCache<K, V> : DbCache where K : IComparable<K>
    {
        readonly bool async;

        // either of two types
        protected readonly IDisposable @lock;

        protected Map<K, V> cached;

        internal DbCache(Func<DbContext, Map<K, V>> fetcher, Type typ, int maxage, byte flag) : base(fetcher, typ, maxage, flag)
        {
            async = false;
            @lock = new ReaderWriterLockSlim();
        }

        internal DbCache(Func<DbContext, Task<Map<K, V>>> fetcher, Type typ, int maxage, byte flag) : base(fetcher, typ, maxage, flag)
        {
            async = true;
            @lock = new SemaphoreSlim(1, 1);
        }

        public override bool IsAsync => async;


        public Map<K, V> Get()
        {
            if (!(fetcher is Func<DbContext, Map<K, V>> func)) // simple object
            {
                throw new DbException("missing fetcher for " + Typ);
            }
            var slim = (ReaderWriterLockSlim) @lock;
            slim.EnterUpgradeableReadLock();
            try
            {
                var tick = Environment.TickCount & int.MaxValue; // positive tick
                if (tick >= expiry) // if re-fetch
                {
                    slim.EnterWriteLock();
                    try
                    {
                        using var dc = Store.NewDbContext();
                        cached = func(dc);
                        expiry = tick + MaxAge * 1000;
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
                return cached;
            }
            finally
            {
                slim.ExitUpgradeableReadLock();
            }
        }

        public async Task<Map<K, V>> GetAsync()
        {
            if (!(fetcher is Func<DbContext, Task<Map<K, V>>> fa)) // simple object
            {
                throw new DbException("Missing fetcher for " + Typ);
            }
            var slim = (SemaphoreSlim) @lock;
            await slim.WaitAsync();
            try
            {
                var ticks = Environment.TickCount & int.MaxValue; // positive tick
                if (ticks >= expiry) // if re-fetch
                {
                    using var dc = Store.NewDbContext();
                    cached = await fa(dc);
                    expiry = ticks + MaxAge * 1000;
                }
            }
            finally
            {
                slim.Release();
            }

            return cached;
        }
    }


    /// <summary>
    /// A cache for multiple objects.
    /// </summary>
    internal class DbObjectCache<K, V> : DbCache
    {
        readonly bool async;

        readonly ConcurrentDictionary<K, V> cached = new ConcurrentDictionary<K, V>();

        internal DbObjectCache(Func<DbContext, K, V> fetcher, Type typ, int maxage, byte flag) : base(fetcher, typ, maxage, flag)
        {
            async = false;
        }

        internal DbObjectCache(Func<DbContext, K, Task<V>> fetcher, Type typ, int maxage, byte flag) : base(fetcher, typ, maxage, flag)
        {
            async = true;
        }

        public override bool IsAsync => async;


        public V Get(K key)
        {
            if (!(fetcher is Func<DbContext, K, V> func)) // simple object
            {
                throw new DbException("missing fetcher for " + Typ);
            }
            var tick = Environment.TickCount & int.MaxValue; // positive tick
            V value;
            if (tick >= expiry) // if re-fetch
            {
                using var dc = Store.NewDbContext();
                value = func(dc, key);
                cached.AddOrUpdate(key, value, (k, old) => old); // re-cache
                expiry = tick + MaxAge * 1000;
            }
            else
            {
                if (!cached.TryGetValue(key, out value))
                {
                    using var dc = Store.NewDbContext();
                    value = func(dc, key);
                    cached.AddOrUpdate(key, value, (k, old) => old); // re-cache
                    expiry = tick + MaxAge * 1000;
                }
            }
            return value;
        }

        public async Task<V> GetAsync(K key)
        {
            if (!(fetcher is Func<DbContext, K, Task<V>> func)) // simple object
            {
                throw new DbException("Missing fetcher for " + Typ);
            }
            var ticks = Environment.TickCount & int.MaxValue; // positive tick
            V value;
            if (ticks >= expiry) // if re-fetch
            {
                using var dc = Store.NewDbContext();
                value = await func(dc, key);
                cached.TryAdd(key, value); // re-cache
                expiry = ticks + MaxAge * 1000;
            }
            else
            {
                cached.TryGetValue(key, out value);
            }
            return value;
        }
    }

    /// <summary>
    /// A cache for multiple maps identified by their keys.
    /// </summary>
    internal class DbMapCache<M, K, V> : DbCache
    {
        readonly bool async;

        // either of two types
        readonly IDisposable @lock;

        readonly ConcurrentDictionary<M, Map<K, V>> cached = new ConcurrentDictionary<M, Map<K, V>>();

        internal DbMapCache(Func<DbContext, M, Map<K, V>> fetcher, Type typ, int maxage, byte flag) : base(fetcher, typ, maxage, flag)
        {
            async = false;
            @lock = new ReaderWriterLockSlim();
        }

        internal DbMapCache(Func<DbContext, M, Task<Map<K, V>>> fetcher, Type typ, int maxage, byte flag) : base(fetcher, typ, maxage, flag)
        {
            async = true;
            @lock = new SemaphoreSlim(1, 1);
        }

        public override bool IsAsync => async;


        public Map<K, V> Get(M mkey)
        {
            if (!(fetcher is Func<DbContext, M, Map<K, V>> func)) // simple object
            {
                throw new DbException("missing fetcher for " + Typ);
            }

            var slim = (ReaderWriterLockSlim) @lock;
            slim.EnterUpgradeableReadLock();
            try
            {
                var ticks = (Environment.TickCount & int.MaxValue); // positive ticks
                Map<K, V> value;
                if (ticks >= expiry) // if re-fetch
                {
                    slim.EnterWriteLock();
                    try
                    {
                        using var dc = Store.NewDbContext();
                        value = func(dc, mkey);
                        cached.TryAdd(mkey, value);
                        // adjust expiry time
                        expiry = ticks + MaxAge * 1000;
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
                else
                {
                    cached.TryGetValue(mkey, out value);
                }
                return value;
            }
            finally
            {
                slim.ExitUpgradeableReadLock();
            }
        }

        public async Task<Map<K, V>> GetAsync(M subkey)
        {
            if (!(fetcher is Func<DbContext, M, Task<Map<K, V>>> func)) // simple object
            {
                throw new DbException("missing fetcher for " + Typ);
            }

            var slim = (SemaphoreSlim) @lock;
            await slim.WaitAsync();
            try
            {
                var ticks = (Environment.TickCount & int.MaxValue); // positive ticks
                Map<K, V> value;
                if (ticks >= expiry) // if re-fetch
                {
                    using var dc = Store.NewDbContext();
                    value = await func(dc, subkey);

                    // adjust expiry time
                    expiry = ticks + MaxAge * 1000;
                }
                else
                {
                    cached.TryGetValue(subkey, out value);
                }
                return value;
            }
            finally
            {
                slim.Release();
            }
        }
    }
}