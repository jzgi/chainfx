using System;
using System.Data;
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

        static DbCache[] cells;

        static int size;

        internal static void ConfigureDb(JObj dbcfg)
        {
            dbsource = new DbSource(dbcfg);
        }

        public static void MakeDbCache<V>(Func<DbContext, V> fetcher, int maxage = 60, byte flag = 0) where V : class
        {
            if (cells == null)
            {
                cells = new DbCache[MAX_CELLS];
            }

            cells[size++] = new DbCache(typeof(V), fetcher, maxage, flag);
        }

        public static void MakeDbCache<V>(Func<DbContext, Task<V>> fetcher, int maxage = 60, byte flag = 0) where V : class
        {
            if (cells == null)
            {
                cells = new DbCache[MAX_CELLS];
            }

            cells[size++] = new DbCache(typeof(V), fetcher, maxage, flag);
        }

        /// <summary>
        /// To obtain a specified cached object..
        /// </summary>
        /// <typeparam name="T">The class must be matched</typeparam>
        /// <returns>the result object or null</returns>
        public static T Obtain<T>(byte flag = 0) where T : class
        {
            if (cells != null)
            {
                for (var i = 0; i < size; i++)
                {
                    var cell = cells[i];
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
    }
}