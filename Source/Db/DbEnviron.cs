using System;
using System.Data;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    public class DbEnviron
    {
        protected static DbSource dbsource;

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
        // Db Cache API
        //

        const int MAX_HOLDS = 32;

        static DbHold[] cache;

        static int size;

        internal static void ConfigureDb(JObj dbcfg)
        {
            dbsource = new DbSource(dbcfg);
        }

        public static void Register<V>(Func<DbContext, V> fetcher, int maxage = 60, byte flag = 0) where V : class
        {
            if (cache == null)
            {
                cache = new DbHold[MAX_HOLDS];
            }

            cache[size++] = new DbHold(typeof(V), fetcher, maxage, flag);
        }

        public static void Register<V>(Func<DbContext, Task<V>> fetcher, int maxage = 60, byte flag = 0) where V : class
        {
            if (cache == null)
            {
                cache = new DbHold[MAX_HOLDS];
            }

            cache[size++] = new DbHold(typeof(V), fetcher, maxage, flag);
        }

        /// <summary>
        /// To obtain a specified cached object..
        /// </summary>
        /// <typeparam name="T">The class must be matched</typeparam>
        /// <returns>the result object or null</returns>
        public static T Obtain<T>(byte flag = 0) where T : class
        {
            if (cache != null)
            {
                for (var i = 0; i < size; i++)
                {
                    var hold = cache[i];
                    if (hold.Flag == 0 || (hold.Flag & flag) > 0)
                    {
                        if (!hold.IsAsync && typeof(T).IsAssignableFrom(hold.Typ))
                        {
                            return hold.GetValue() as T;
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
            if (cache != null)
            {
                for (int i = 0; i < size; i++)
                {
                    var hold = cache[i];
                    if (hold.Flag == 0 || (hold.Flag & flag) > 0)
                    {
                        if (hold.IsAsync && typeof(T).IsAssignableFrom(hold.Typ))
                        {
                            return await hold.GetValueAsync() as T;
                        }
                    }
                }
            }

            return null;
        }
    }
}