using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SkyCloud.Db
{
    public class Db
    {
        protected static DbSource dbsource;

        //
        // db-object cache
        //

        public static DbSource DbSource() => dbsource;

        public static DbContext NewDbContext(IsolationLevel? level = null)
        {
            if (dbsource == null)
            {
                throw new FrameworkException("missing 'db' in app.json");
            }

            return dbsource.NewDbContext(level);
        }

        static Cell[] cells;

        static int size;

        static readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        public static void Cache(object value, byte flag = 0)
        {
            if (cells == null)
            {
                cells = new Cell[32];
            }

            cells[size++] = new Cell(value, flag);
        }

        public static void Cache<V>(Func<DbContext, V> fetch, int maxage = 60, byte flag = 0) where V : class
        {
            if (cells == null)
            {
                cells = new Cell[8];
            }

            cells[size++] = new Cell(typeof(V), fetch, maxage, flag);
        }

        public static void Cache<V>(Func<DbContext, Task<V>> fetchAsync, int maxage = 60, byte flag = 0) where V : class
        {
            if (cells == null)
            {
                cells = new Cell[8];
            }

            cells[size++] = new Cell(typeof(V), fetchAsync, maxage, flag);
        }

        /// <summary>
        /// Search for typed object in this scope and the scopes of ancestors; 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>the result object or null</returns>
        public static T Obtain<T>(byte flag = 0) where T : class
        {
            if (cells != null)
            {
                for (int i = 0; i < size; i++)
                {
                    var c = cells[i];
                    if (c.Flag == 0 || (c.Flag & flag) > 0)
                    {
                        if (!c.IsAsync && typeof(T).IsAssignableFrom(c.Typ))
                        {
                            return c.GetValue() as T;
                        }
                    }
                }
            }

            return null;
        }

        public static async Task<T> ObtainAsync<T>(byte flag = 0) where T : class
        {
            if (cells != null)
            {
                for (int i = 0; i < size; i++)
                {
                    var cell = cells[i];
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

            readonly Func<DbContext, object> fetch;

            readonly Func<DbContext, Task<object>> fetchAsync;

            readonly int maxage; //in seconds

            // tick count,   
            int expiry;

            object value;

            readonly byte flag;

            internal Cell(object value, byte flag)
            {
                this.typ = value.GetType();
                this.value = value;
                this.flag = flag;
            }

            internal Cell(Type typ, Func<DbContext, object> fetch, int maxage, byte flag)
            {
                this.typ = typ;
                this.flag = flag;
                if (fetch is Func<DbContext, Task<object>> fetch2)
                {
                    this.fetchAsync = fetch2;
                }
                else
                {
                    this.fetch = fetch;
                }

                this.maxage = maxage;
            }

            public Type Typ => typ;

            public byte Flag => flag;

            public bool IsAsync => fetchAsync != null;

            public object GetValue()
            {
                if (fetch == null) // simple object
                {
                    return value;
                }

                @lock.EnterUpgradeableReadLock();
                try
                {
                    if (Environment.TickCount >= expiry)
                    {
                        @lock.EnterWriteLock();
                        try
                        {
                            using var dc = NewDbContext();
                            value = fetch(dc);
                            expiry = (Environment.TickCount & int.MaxValue) + maxage * 1000;
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
                if (fetchAsync == null) // simple object
                {
                    return value;
                }

                int lexpiry = this.expiry;
                int ticks = Environment.TickCount;
                if (ticks >= lexpiry)
                {
                    using var dc = NewDbContext();
                    value = await fetchAsync(dc);
                    expiry = (Environment.TickCount & int.MaxValue) + maxage * 1000;
                }

                return value;
            }
        }
    }
}