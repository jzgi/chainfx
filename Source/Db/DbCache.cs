using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    internal abstract class DbCache
    {
        // typ to seek for
        readonly Type typ;

        // bitwise matcher
        readonly byte flag;

        // in seconds
        readonly int maxage;

        // either of two forms
        protected readonly Delegate fetcher;

        // tick count,   
        protected int expiry;


        protected DbCache(Delegate fetcher, Type typ, int maxage, byte flag)
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

        public byte Flag => flag;
    }


    /// <summary>
    /// A single cached object.
    /// </summary>
    internal class DbMap<K, V> : DbCache where K : IComparable<K>
    {
        readonly bool async;

        // either of two types
        protected readonly IDisposable @lock;

        protected Map<K, V> cached;

        internal DbMap(Func<DbContext, Map<K, V>> fetcher, Type typ, int maxage, byte flag)
            : base(fetcher, typ, maxage, flag)
        {
            async = false;
            @lock = new ReaderWriterLockSlim();
        }

        internal DbMap(Func<DbContext, Task<Map<K, V>>> fetcher, Type typ, int maxage, byte flag)
            : base(fetcher, typ, maxage, flag)
        {
            async = true;
            @lock = new SemaphoreSlim(1, 1);
        }

        public override bool IsAsync => async;


        public Map<K, V> Get()
        {
            if (!(fetcher is Func<DbContext, Map<K, V>> func)) // simple object
            {
                throw new DbException("Missing fetcher for " + Typ);
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
                        using var dc = DbEnv.NewDbContext();
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
                    using var dc = DbEnv.NewDbContext();
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


    internal class DbValueSet<K, V> : DbCache
    {
        readonly bool async;

        readonly ConcurrentDictionary<K, V> cached = new ConcurrentDictionary<K, V>();

        internal DbValueSet(Func<DbContext, K, V> fetcher, Type typ, int maxage, byte flag)
            : base(fetcher, typ, maxage, flag)
        {
            async = false;
        }

        internal DbValueSet(Func<DbContext, K, Task<V>> fetcher, Type typ, int maxage, byte flag)
            : base(fetcher, typ, maxage, flag)
        {
            async = true;
        }

        public override bool IsAsync => async;


        public V Get(K key)
        {
            if (!(fetcher is Func<DbContext, K, V> func)) // simple object
            {
                throw new DbException("Missing fetcher for " + Typ);
            }
            var tick = Environment.TickCount & int.MaxValue; // positive tick
            V value;
            if (tick >= expiry) // if re-fetch
            {
                using var dc = DbEnv.NewDbContext();
                value = func(dc, key);
                cached.AddOrUpdate(key, value, (k, old) => old); // re-cache
                expiry = tick + MaxAge * 1000;
            }
            else
            {
                if (!cached.TryGetValue(key, out value))
                {
                    using var dc = DbEnv.NewDbContext();
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
                using var dc = DbEnv.NewDbContext();
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
    /// A subbed or segmented cache..
    /// </summary>
    /// <typeparam name="S">the sub key</typeparam>
    /// <typeparam name="K"> </typeparam>
    /// <typeparam name="V">the cached object</typeparam>
    internal class DbMapSet<S, K, V> : DbCache
    {
        readonly bool async;

        // either of two types
        readonly IDisposable @lock;

        readonly ConcurrentDictionary<S, Map<K, V>> cached = new ConcurrentDictionary<S, Map<K, V>>();

        internal DbMapSet(Func<DbContext, S, Map<K, V>> fetcher, Type typ, int maxage, byte flag)
            : base(fetcher, typ, maxage, flag)
        {
            async = false;
            @lock = new ReaderWriterLockSlim();
        }

        internal DbMapSet(Func<DbContext, S, Task<Map<K, V>>> fetcher, Type typ, int maxage, byte flag)
            : base(fetcher, typ, maxage, flag)
        {
            async = true;
            @lock = new SemaphoreSlim(1, 1);
        }

        public override bool IsAsync => async;


        public Map<K, V> Get(S subkey)
        {
            if (!(fetcher is Func<DbContext, S, Map<K, V>> func)) // simple object
            {
                throw new DbException("Missing fetcher for " + Typ);
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
                        using var dc = DbEnv.NewDbContext();
                        value = func(dc, subkey);
                        cached.TryAdd(subkey, value);
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
                    cached.TryGetValue(subkey, out value);
                }
                return value;
            }
            finally
            {
                slim.ExitUpgradeableReadLock();
            }
        }

        public async Task<Map<K, V>> GetAsync(S subkey)
        {
            if (!(fetcher is Func<DbContext, S, Task<Map<K, V>>> func)) // simple object
            {
                throw new DbException("Missing fetcher for " + Typ);
            }

            var slim = (SemaphoreSlim) @lock;
            await slim.WaitAsync();
            try
            {
                var ticks = (Environment.TickCount & int.MaxValue); // positive ticks
                Map<K, V> value;
                if (ticks >= expiry) // if re-fetch
                {
                    using var dc = DbEnv.NewDbContext();
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