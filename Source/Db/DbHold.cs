using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    internal abstract class DbHold
    {
        readonly Type typ;

        // in seconds
        readonly int maxage;

        // bitwise matcher
        readonly byte flag;

        // tick count,   
        protected int expiry;

        protected readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        protected readonly SemaphoreSlim lockAsync = new SemaphoreSlim(1, 1);


        protected DbHold(Type typ, int maxage, byte flag)
        {
            this.typ = typ;
            this.maxage = maxage;
            this.flag = flag;
        }

        public Type Typ => typ;

        public int MaxAge => maxage;

        public byte Flag => flag;

        public abstract bool IsAsync { get; }
    }


    /// <summary>
    /// An entry of object cache.
    /// </summary>
    internal class DbSimpleHold<V> : DbHold
    {
        V value;

        internal Func<DbContext, V> Fetcher { get; set; }

        internal Func<DbContext, Task<V>> FetcherAsync { get; set; }


        internal DbSimpleHold(Type typ, int maxage, byte flag) : base(typ, maxage, flag)
        {
        }


        public override bool IsAsync => FetcherAsync != null;

        public V GetValue()
        {
            if (Fetcher == null) // simple object
            {
                throw new DbException("missong fetcher (sync)");
            }
            @lock.EnterUpgradeableReadLock();
            try
            {
                var ptick = (Environment.TickCount & Int32.MaxValue); // positive tick
                if (ptick >= expiry) // refetch
                {
                    @lock.EnterWriteLock();
                    try
                    {
                        using var dc = DbEnviron.NewDbContext();
                        value = Fetcher(dc);
                        expiry = ptick + MaxAge * 1000;
                    }
                    finally
                    {
                        @lock.ExitWriteLock();
                    }
                }
                return value;
            }
            finally
            {
                @lock.ExitUpgradeableReadLock();
            }
        }

        public async Task<V> GetValueAsync()
        {
            if (FetcherAsync == null) // simple object
            {
                throw new DbException("missong fetcher (async)");
            }
            await lockAsync.WaitAsync();
            try
            {
                int lexpiry = this.expiry;
                int ticks = Environment.TickCount;
                if (ticks >= lexpiry)
                {
                    using var dc = DbEnviron.NewDbContext();
                    value = await FetcherAsync(dc);
                    expiry = (Environment.TickCount & Int32.MaxValue) + MaxAge * 1000;
                }
            }
            finally
            {
                lockAsync.Release();
            }

            return value;
        }
    }

    /// <summary>
    /// Eaach hold is a collection of objects.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    internal class DbComplexHold<K, V> : DbHold
    {
        readonly ConcurrentDictionary<K, V> values;

        internal Func<DbContext, (K, V)> Fetcher { get; set; }

        internal Func<DbContext, Task<(K, V)>> FetcherAsync { get; set; }


        internal DbComplexHold(Type typ, int maxage, byte flag) : base(typ, maxage, flag)
        {
            values = new ConcurrentDictionary<K, V>();
        }


        public override bool IsAsync => FetcherAsync != null;

        public V GetValue(K key)
        {
            if (Fetcher == null) // simple object
            {
                throw new DbException("missong simple fetcher");
            }
            @lock.EnterUpgradeableReadLock();
            try
            {
                var value = values[key];
                var ptick = (Environment.TickCount & Int32.MaxValue); // positive tick
                if (value == null || ptick >= expiry) // re-fetch from db
                {
                    @lock.EnterWriteLock();
                    try
                    {
                        using var dc = DbEnviron.NewDbContext();
                        (_, value) = Fetcher(dc);
                        values.AddOrUpdate(key, value, (x, o) => o);

                        // adjust expiry time
                        expiry = ptick + MaxAge * 1000;
                    }
                    finally
                    {
                        @lock.ExitWriteLock();
                    }
                }
                return value;
            }
            finally
            {
                @lock.ExitUpgradeableReadLock();
            }
        }

        public async Task<V> GetValueAsync(K key)
        {
            if (FetcherAsync == null) // simple object
            {
                throw new DbException("missong async fetcher");
            }
            await lockAsync.WaitAsync();
            try
            {
                var value = values[key];
                var ptick = (Environment.TickCount & Int32.MaxValue); // positive tick
                if (value == null || ptick >= expiry) // re-fetch from db
                {
                    using var dc = DbEnviron.NewDbContext();
                    (_, value) = Fetcher(dc);
                    values.AddOrUpdate(key, value, (x, old) => old);

                    // adjust expiry time
                    expiry = ptick + MaxAge * 1000;
                }
                return value;
            }
            finally
            {
                lockAsync.Release();
            }
        }
    }
}