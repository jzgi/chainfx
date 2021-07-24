using System;
using System.Threading;
using System.Threading.Tasks;
using SkyChain.Db;

namespace SkyChain.Db
{
    /// <summary>
    /// A object holder in registry.
    /// </summary>
    class DbCache
    {
        readonly Type typ;

        readonly Func<DbContext, object> fetcher;

        static readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        readonly Func<DbContext, Task<object>> fetcherA;

        static readonly SemaphoreSlim lockA = new SemaphoreSlim(1, 1);

        readonly int maxage; //in seconds

        // tick count,   
        int expiry;

        object value;

        readonly byte flag;

        internal DbCache(Type typ, Func<DbContext, object> fetcher, int maxage, byte flag)
        {
            this.typ = typ;
            this.flag = flag;
            if (fetcher is Func<DbContext, Task<object>> func)
            {
                this.fetcherA = func;
            }
            else
            {
                this.fetcher = fetcher;
            }

            this.maxage = maxage;
        }

        public Type Typ => typ;

        public byte Flag => flag;

        public bool IsAsync => fetcherA != null;

        public object GetValue()
        {
            if (fetcher == null) // simple object
            {
                return value;
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
                        value = fetcher(dc);
                        expiry = ptick + maxage * 1000;
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

        public async Task<object> GetValueAsync()
        {
            if (fetcherA == null) // simple object
            {
                return value;
            }
            await lockA.WaitAsync();
            try
            {
                int lexpiry = this.expiry;
                int ticks = Environment.TickCount;
                if (ticks >= lexpiry)
                {
                    using var dc = DbEnviron.NewDbContext();
                    value = await fetcherA(dc);
                    expiry = (Environment.TickCount & Int32.MaxValue) + maxage * 1000;
                }
            }
            finally
            {
                lockA.Release();
            }

            return value;
        }
    }
}