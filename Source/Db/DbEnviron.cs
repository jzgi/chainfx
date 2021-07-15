using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    public class DbEnviron
    {
        static DbSource dbsource;

        //
        // db-object cache
        //

        public static DbSource DbSource => dbsource;

        public static DbContext NewDbContext(IsolationLevel? level = null)
        {
            if (dbsource == null)
            {
                throw new ServerException("missing 'db' in app.json");
            }

            return dbsource.NewDbContext(level);
        }

        //
        // Object Cache API
        //

        const int MAX_CELLS = 32;

        static Cell[] registry;

        static int size;

        internal static void ConfigureDb(JObj dbcfg)
        {
            dbsource = new DbSource(dbcfg);
        }

        public static void Register<V>(Func<DbContext, V> fetcher, int maxage = 60, byte flag = 0) where V : class
        {
            if (registry == null)
            {
                registry = new Cell[MAX_CELLS];
            }

            registry[size++] = new Cell(typeof(V), fetcher, maxage, flag);
        }

        public static void Register<V>(Func<DbContext, Task<V>> fetcher, int maxage = 60, byte flag = 0) where V : class
        {
            if (registry == null)
            {
                registry = new Cell[MAX_CELLS];
            }

            registry[size++] = new Cell(typeof(V), fetcher, maxage, flag);
        }

        /// <summary>
        /// To obtain a specified cached object..
        /// </summary>
        /// <typeparam name="T">The class must be matched</typeparam>
        /// <returns>the result object or null</returns>
        public static T Obtain<T>(byte flag = 0) where T : class
        {
            if (registry != null)
            {
                for (var i = 0; i < size; i++)
                {
                    var cell = registry[i];
                    if (cell.Flag == 0 || (cell.Flag & flag) > 0)
                    {
                        if (!cell.IsAsync && typeof(T).IsAssignableFrom(cell.Typ))
                        {
                            return cell.GetValue() as T;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// To obtain a specified cached object asynchromously.
        /// </summary>
        /// <param name="flag"></param>
        /// <typeparam name="T">The class must be matched</typeparam>
        /// <returns></returns>
        public static async Task<T> ObtainAsync<T>(byte flag = 0) where T : class
        {
            if (registry != null)
            {
                for (int i = 0; i < size; i++)
                {
                    var cell = registry[i];
                    if (cell.Flag == 0 || (cell.Flag & flag) > 0)
                    {
                        if (cell.IsAsync && typeof(T).IsAssignableFrom(cell.Typ))
                        {
                            return await cell.GetValueAsync() as T;
                        }
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// A object holder in registry.
        /// </summary>
        class Cell
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

            internal Cell(Type typ, Func<DbContext, object> fetcher, int maxage, byte flag)
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
                    var ptick = (Environment.TickCount & int.MaxValue); // positive tick
                    if (ptick >= expiry) // refetch
                    {
                        @lock.EnterWriteLock();
                        try
                        {
                            using var dc = NewDbContext();
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
                        using var dc = NewDbContext();
                        value = await fetcherA(dc);
                        expiry = (Environment.TickCount & int.MaxValue) + maxage * 1000;
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
}