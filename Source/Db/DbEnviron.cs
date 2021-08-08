using System;
using System.Collections.Generic;
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

        static List<DbCache> objcaches;

        static List<DbCache> dictcaches;


        internal static void ConfigureDb(JObj dbcfg)
        {
            dbsource = new DbSource(dbcfg);
        }

        public static void MakeCache<T>(Func<DbContext, T> fetcher, int maxage = 60, byte flag = 0) where T : class
        {
            if (objcaches == null)
            {
                objcaches = new List<DbCache>(16);
            }
            objcaches.Add(new DbCache<T>(typeof(T), fetcher, maxage, flag));
        }

        public static void MakeCache<T>(Func<DbContext, Task<T>> fetcher, int maxage = 60, byte flag = 0) where T : class
        {
            if (objcaches == null)
            {
                objcaches = new List<DbCache>(16);
            }
            objcaches.Add(new DbCache<T>(typeof(T), fetcher, maxage, flag));
        }


        // public static void MakeCache<T>(Func<DbContext, Task<T>> fetcher, int maxage = 60, byte flag = 0) where T : class
        // {
        // }

        public static void MakeCache<D, T>(Func<DbContext, D, T> fetcher, int maxage = 60, byte flag = 0) where T : class
        {
            if (dictcaches == null)
            {
                dictcaches = new List<DbCache>();
            }
            dictcaches.Add(new DbCache<D, T>(typeof(T), fetcher, maxage, flag));
        }

        public static void MakeCache<D, T>(Func<DbContext, D, Task<T>> fetcher, int maxage = 60, byte flag = 0) where T : class
        {
            if (dictcaches == null)
            {
                dictcaches = new List<DbCache>();
            }
            dictcaches.Add(new DbCache<D, T>(typeof(T), fetcher, maxage, flag));
        }


        /// <summary>
        /// To obtain a specified cached object..
        /// </summary>
        /// <typeparam name="T">The class must be matched</typeparam>
        /// <returns>the result object or null</returns>
        public static T Fetch<T>(byte flag = 0) where T : class
        {
            if (objcaches == null)
            {
                return null;
            }
            foreach (var cache in objcaches)
            {
                if (cache.Flag == 0 || (cache.Flag & flag) > 0)
                {
                    if (!cache.IsAsync && typeof(T).IsAssignableFrom(cache.Typ))
                    {
                        return ((DbCache<T>) cache).GetValue();
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
        public static async Task<T> FetchAsync<T>(byte flag = 0) where T : class
        {
            if (objcaches == null)
            {
                return null;
            }
            foreach (var cache in objcaches)
            {
                if (cache.Flag == 0 || (cache.Flag & flag) > 0)
                {
                    if (cache.IsAsync && typeof(T).IsAssignableFrom(cache.Typ))
                    {
                        return await ((DbCache<T>) cache).GetValueAsync();
                    }
                }
            }

            return null;
        }

        public static T Fetch<D, T>(D discr, byte flag = 0) where T : class
        {
            if (dictcaches == null)
            {
                return null;
            }
            foreach (var cache in dictcaches)
            {
                if (cache.Flag == 0 || (cache.Flag & flag) > 0)
                {
                    if (!cache.IsAsync && typeof(T).IsAssignableFrom(cache.Typ))
                    {
                        return ((DbCache<D, T>) cache).GetValue(discr);
                    }
                }
            }

            return null;
        }

        public static async Task<T> FetchAsync<D, T>(D discr, byte flag = 0) where T : class
        {
            if (dictcaches == null)
            {
                return null;
            }
            foreach (var cache in dictcaches)
            {
                if (cache.Flag == 0 || (cache.Flag & flag) > 0)
                {
                    if (cache.IsAsync && typeof(T).IsAssignableFrom(cache.Typ))
                    {
                        return await ((DbCache<D, T>) cache).GetValueAsync(discr);
                    }
                }
            }
            return null;
        }
    }
}