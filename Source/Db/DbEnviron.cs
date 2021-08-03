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

        const int MAX_HOLDS = 32;

        static List<DbHold> simples;

        static List<DbHold> complexes;

        static int size;

        internal static void ConfigureDb(JObj dbcfg)
        {
            dbsource = new DbSource(dbcfg);
        }

        public static void SetCache<V>(Func<DbContext, V> fetcher, int maxage = 60, byte flag = 0) where V : class
        {
            if (simples == null)
            {
                simples = new List<DbHold>(16);
            }
            simples.Add(new DbSimpleHold<V>(typeof(V), maxage, flag)
            {
                Fetcher = fetcher
            });
        }

        public static void SetCache<V>(Func<DbContext, Task<V>> fetcherAsync, int maxage = 60, byte flag = 0) where V : class
        {
            if (simples == null)
            {
                simples = new List<DbHold>(16);
            }
            simples.Add(new DbSimpleHold<V>(typeof(V), maxage, flag)
            {
                FetcherAsync = fetcherAsync
            });
        }

        public static void SetCache<K, V>(Func<DbContext, (K, V)> fetcher, int maxage = 60, byte flag = 0) where V : class
        {
            if (complexes == null)
            {
                complexes = new List<DbHold>();
            }
            complexes.Add(new DbComplexHold<K, V>(typeof(V), maxage, flag)
            {
                Fetcher = fetcher
            });
        }

        public static void SetCache<K, V>(Func<DbContext, Task<(K, V)>> fetcherAsync, int maxage = 60, byte flag = 0) where V : class
        {
            if (complexes == null)
            {
                complexes = new List<DbHold>();
            }
            complexes.Add(new DbComplexHold<K, V>(typeof(V), maxage, flag)
            {
                FetcherAsync = fetcherAsync
            });
        }


        /// <summary>
        /// To obtain a specified cached object..
        /// </summary>
        /// <typeparam name="T">The class must be matched</typeparam>
        /// <returns>the result object or null</returns>
        public static T Fetch<T>(byte flag = 0) where T : class
        {
            if (simples != null)
            {
                for (var i = 0; i < size; i++)
                {
                    var hold = simples[i];
                    if (hold.Flag == 0 || (hold.Flag & flag) > 0)
                    {
                        if (!hold.IsAsync && typeof(T).IsAssignableFrom(hold.Typ))
                        {
                            return ((DbSimpleHold<T>) hold).GetValue();
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
        /// <typeparam name="V">The class must be matched</typeparam>
        /// <returns></returns>
        public static async Task<V> FetchAsync<V>(byte flag = 0) where V : class
        {
            if (simples != null)
            {
                for (int i = 0; i < size; i++)
                {
                    var hold = simples[i];
                    if (hold.Flag == 0 || (hold.Flag & flag) > 0)
                    {
                        if (hold.IsAsync && typeof(V).IsAssignableFrom(hold.Typ))
                        {
                            return await ((DbSimpleHold<V>) hold).GetValueAsync();
                        }
                    }
                }
            }

            return null;
        }

        public static T Fetch<K, T>(K key, byte flag = 0) where T : class
        {
            if (simples != null)
            {
                for (var i = 0; i < size; i++)
                {
                    var hold = complexes[i];
                    if (hold.Flag == 0 || (hold.Flag & flag) > 0)
                    {
                        if (!hold.IsAsync && typeof(T).IsAssignableFrom(hold.Typ))
                        {
                            return ((DbComplexHold<K, T>) hold).GetValue(key);
                        }
                    }
                }
            }

            return null;
        }
    }
}