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


        protected DbCache(Type typ, int maxage, byte flag)
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
    /// A single cached object.
    /// </summary>
    internal class DbCache<T> : DbCache
    {
        readonly bool async;

        // either of two forms
        readonly Delegate fetcher;

        // either of two types
        readonly IDisposable @lock;

        // tick count,   
        int expiry;

        T value;


        internal DbCache(Type typ, Func<DbContext, T> fetcher, int maxage, byte flag)
            : base(typ, maxage, flag)
        {
            this.fetcher = fetcher;
            this.async = false;
            this.@lock = new ReaderWriterLockSlim();
        }

        internal DbCache(Type typ, Func<DbContext, Task<T>> fetcher, int maxage, byte flag)
            : base(typ, maxage, flag)
        {
            this.fetcher = fetcher;
            this.async = true;
            this.@lock = new SemaphoreSlim(1, 1);
        }

        public override bool IsAsync => async;


        public T GetValue()
        {
            if (!(fetcher is Func<DbContext, T> f)) // simple object
            {
                throw new DbException("missong cache fetcher for " + Typ);
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
                        using var dc = DbEnviron.NewDbContext();
                        value = f(dc);
                        expiry = tick + MaxAge * 1000;
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
                return value;
            }
            finally
            {
                slim.ExitUpgradeableReadLock();
            }
        }

        public async Task<T> GetValueAsync()
        {
            if (!(fetcher is Func<DbContext, Task<T>> fa)) // simple object
            {
                throw new DbException("missong async cache fetcher for " + Typ);
            }
            var slim = (SemaphoreSlim) @lock;
            await slim.WaitAsync();
            try
            {
                var ticks = Environment.TickCount & int.MaxValue; // positive tick
                if (ticks >= expiry) // if re-fetch
                {
                    using var dc = DbEnviron.NewDbContext();
                    value = await fa(dc);
                    expiry = ticks + MaxAge * 1000;
                }
            }
            finally
            {
                slim.Release();
            }

            return value;
        }
    }

    /// <summary>
    /// A keyed collection of cached objects.
    /// </summary>
    /// <typeparam name="D">the discriminator key</typeparam>
    /// <typeparam name="T">the cached object</typeparam>
    internal class DbCache<D, T> : DbCache
    {
        readonly bool async;

        readonly Delegate fetcher;

        readonly ConcurrentDictionary<D, Cell> cells;

        internal DbCache(Type typ, Func<DbContext, D, T> fetcher, int maxage, byte flag)
            : base(typ, maxage, flag)
        {
            cells = new ConcurrentDictionary<D, Cell>();
            this.fetcher = fetcher;
            this.async = false;
        }

        internal DbCache(Type typ, Func<DbContext, D, Task<T>> fetcher, int maxage, byte flag)
            : base(typ, maxage, flag)
        {
            cells = new ConcurrentDictionary<D, Cell>();
            this.fetcher = fetcher;
            this.async = true;
        }


        public override bool IsAsync => async;


        public T GetValue(D discr)
        {
            if (!(fetcher is Func<DbContext, D, T> f)) // simple object
            {
                throw new DbException("missong cache fetcher for " + Typ);
            }
            // locate the cell, or create new
            if (!cells.TryGetValue(discr, out var cell))
            {
                cell = new Cell(async);
                cells.AddOrUpdate(discr, cell, (k, old) => old);
            }

            var slim = (ReaderWriterLockSlim) cell.@lock;
            slim.EnterUpgradeableReadLock();
            try
            {
                var ticks = (Environment.TickCount & int.MaxValue); // positive ticks
                if (ticks >= cell.expiry) // if re-fetch
                {
                    slim.EnterWriteLock();
                    try
                    {
                        using var dc = DbEnviron.NewDbContext();
                        cell.value = f(dc, discr);

                        // adjust expiry time
                        cell.expiry = ticks + MaxAge * 1000;
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
                return cell.value;
            }
            finally
            {
                slim.ExitUpgradeableReadLock();
            }
        }

        public async Task<T> GetValueAsync(D discr)
        {
            if (!(fetcher is Func<DbContext, D, Task<T>> fa)) // simple object
            {
                throw new DbException("missong cache fetcher for " + Typ);
            }
            // locate the cell, or create new
            if (!cells.TryGetValue(discr, out var cell))
            {
                cell = new Cell(async);
                cells.AddOrUpdate(discr, cell, (k, old) => old);
            }

            var slim = (SemaphoreSlim) cell.@lock;
            await slim.WaitAsync();
            try
            {
                var ticks = (Environment.TickCount & int.MaxValue); // positive ticks
                if (ticks >= cell.expiry) // if re-fetch
                {
                    using var dc = DbEnviron.NewDbContext();
                    cell.value = await fa(dc, discr);

                    // adjust expiry time
                    cell.expiry = ticks + MaxAge * 1000;
                }
                return cell.value;
            }
            finally
            {
                slim.Release();
            }
        }

        class Cell
        {
            internal readonly IDisposable @lock;

            // tick count,   
            internal int expiry;

            internal T value;

            public Cell(bool async)
            {
                this.@lock = async ? (IDisposable) new SemaphoreSlim(1, 1) : new ReaderWriterLockSlim();
            }
        }
    }
}