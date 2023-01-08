using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ChainFx.Fabric
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
    /// A cache for single entire map.
    /// </summary>
    internal class DbCache<K, V> : DbCache where K : IComparable<K>
    {
        readonly bool async;

        // either readerwriterlock or semaphore
        protected readonly IDisposable slim;

        protected Map<K, V> cached;

        // tick count,   
        volatile int expiry;


        internal DbCache(Func<DbContext, Map<K, V>> fetcher, Type typ, int maxage, byte flag) : base(fetcher, typ, maxage, flag)
        {
            async = false;
            slim = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// The async version of constructor.
        /// </summary>
        /// <param name="fetcher"></param>
        /// <param name="typ"></param>
        /// <param name="maxage"></param>
        /// <param name="flag"></param>
        internal DbCache(Func<DbContext, Task<Map<K, V>>> fetcher, Type typ, int maxage, byte flag) : base(fetcher, typ, maxage, flag)
        {
            async = true;
            slim = new SemaphoreSlim(1, 1);
        }

        public override bool IsAsync => async;


        public Map<K, V> Get()
        {
            if (!(fetcher is Func<DbContext, Map<K, V>> func)) // check fetcher
            {
                throw new DbException("Missing fetcher for " + Typ);
            }
            var @lock = (ReaderWriterLockSlim) slim;
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
            if (!(fetcher is Func<DbContext, Task<Map<K, V>>> func)) // check fetcher
            {
                throw new DbException("Wrong fetcher for " + Typ);
            }
            var semaphore = (SemaphoreSlim) slim;
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


    /// <summary>
    /// A cache for multiple objects.
    /// </summary>
    internal class DbObjectCache<K, V> : DbCache
    {
        readonly bool async;

        readonly ConcurrentDictionary<K, (int, V)> cached = new ConcurrentDictionary<K, (int, V)>();

        /// <summary>
        /// The sync version of constructor.
        /// </summary>
        /// <param name="fetcher"></param>
        /// <param name="typ"></param>
        /// <param name="maxage"></param>
        /// <param name="flag"></param>
        internal DbObjectCache(Func<DbContext, K, V> fetcher, Type typ, int maxage, byte flag) : base(fetcher, typ, maxage, flag)
        {
            async = false;
        }

        /// <summary>
        /// The async version of constructor.
        /// </summary>
        /// <param name="fetcher"></param>
        /// <param name="typ"></param>
        /// <param name="maxage"></param>
        /// <param name="flag"></param>
        internal DbObjectCache(Func<DbContext, K, Task<V>> fetcher, Type typ, int maxage, byte flag) : base(fetcher, typ, maxage, flag)
        {
            async = true;
        }

        public override bool IsAsync => async;


        public V Get(K key)
        {
            if (!(fetcher is Func<DbContext, K, V> func)) // check fetcher
            {
                throw new DbException("Wrong fetcher for " + Typ);
            }
            var tick = Environment.TickCount & int.MaxValue; // positive tick

            var exist = cached.TryGetValue(key, out var ety);
            if (exist && tick < ety.Item1)
            {
                return ety.Item2;
            }

            // re-fetch
            using var dc = Nodality.NewDbContext();
            ety.Item1 = tick + MaxAge * 1000;
            ety.Item2 = func(dc, key);

            cached.AddOrUpdate(key, ety, (k, old) => ety); // re-cache

            return ety.Item2;
        }

        public async Task<V> GetAsync(K key)
        {
            if (!(fetcher is Func<DbContext, K, Task<V>> func)) // check fetcher
            {
                throw new DbException("Wrong fetcher for " + Typ);
            }
            var tick = Environment.TickCount & int.MaxValue; // positive tick

            var exist = cached.TryGetValue(key, out var ety);
            if (exist && tick < ety.Item1)
            {
                return ety.Item2;
            }

            // re-fetch
            using var dc = Nodality.NewDbContext();
            ety.Item1 = tick + MaxAge * 1000;
            ety.Item2 = await func(dc, key);

            cached.AddOrUpdate(key, ety, (k, old) => ety); // re-cache

            return ety.Item2;
        }
    }

    /// <summary>
    /// A cache for multiple maps identified by their keys.
    /// </summary>
    internal class DbMapCache<M, K, V> : DbCache
    {
        readonly bool async;

        readonly ConcurrentDictionary<M, (int, Map<K, V>)> cached = new ConcurrentDictionary<M, (int, Map<K, V>)>();


        internal DbMapCache(Func<DbContext, M, Map<K, V>> fetcher, Type typ, int maxage, byte flag) : base(fetcher, typ, maxage, flag)
        {
            async = false;
        }

        internal DbMapCache(Func<DbContext, M, Task<Map<K, V>>> fetcher, Type typ, int maxage, byte flag) : base(fetcher, typ, maxage, flag)
        {
            async = true;
        }

        public override bool IsAsync => async;


        public Map<K, V> Get(M key)
        {
            if (!(fetcher is Func<DbContext, M, Map<K, V>> func)) // simple object
            {
                throw new DbException("Wrong fetcher for " + Typ);
            }

            var tick = Environment.TickCount & int.MaxValue; // positive tick

            var exist = cached.TryGetValue(key, out var ety);
            if (exist && tick < ety.Item1)
            {
                return ety.Item2;
            }

            // re-fetch
            using var dc = Nodality.NewDbContext();
            ety.Item1 = tick + MaxAge * 1000;
            ety.Item2 = func(dc, key);

            cached.AddOrUpdate(key, ety, (k, old) => ety);

            return ety.Item2;
        }

        public async Task<Map<K, V>> GetAsync(M key)
        {
            if (!(fetcher is Func<DbContext, M, Task<Map<K, V>>> func)) // check fetcher
            {
                throw new DbException("Wrong fetcher for " + Typ);
            }

            var tick = Environment.TickCount & int.MaxValue; // positive tick

            var exist = cached.TryGetValue(key, out var ety);
            if (exist && tick < ety.Item1)
            {
                return ety.Item2;
            }

            // re-fetch
            using var dc = Nodality.NewDbContext();
            ety.Item1 = tick + MaxAge * 1000;
            ety.Item2 = await func(dc, key);

            cached.AddOrUpdate(key, ety, (k, old) => ety);

            return ety.Item2;
        }
    }
}