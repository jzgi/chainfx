using System;
using System.Collections.Generic;
using System.Threading;

namespace Greatbone.Core
{
    /// <summary>
    /// A caching key/value dataset that periodically reloads.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class Roll<K, V> : ReaderWriterLockSlim
    {
        readonly Func<Map<K, V>> loader;

        Exception excep;

        readonly int age; //in seconds

        Map<K, V> map;

        // tick count,   
        int expiry;

        public Roll(Func<Map<K, V>> loader, int age = 3600 * 12)
        {
            this.age = age;
            this.loader = loader;
        }

        public int Age => age;

        public Exception Excep => excep;

        public V this[K key]
        {
            get
            {
                EnterUpgradeableReadLock();
                try
                {
                    if (Environment.TickCount >= expiry)
                    {
                        EnterWriteLock();
                        try
                        {
                            map = loader();
                        }
                        catch (Exception ex)
                        {
                            excep = ex;
                        }
                        finally
                        {
                            expiry = (Environment.TickCount & int.MaxValue) + age * 1000;
                            ExitWriteLock();
                        }
                    }
                    return map[key];
                }
                finally
                {
                    ExitUpgradeableReadLock();
                }
            }
        }

        public V First(Predicate<V> cond = null)
        {
            EnterUpgradeableReadLock();
            try
            {
                if (map == null || (Environment.TickCount & int.MaxValue) >= expiry) // whether to load
                {
                    EnterWriteLock();
                    try
                    {
                        map = loader();
                    }
                    catch (Exception ex)
                    {
                        excep = ex;
                    }
                    finally
                    {
                        expiry = (Environment.TickCount & int.MaxValue) + age * 1000;
                        ExitWriteLock();
                    }
                }
                // search
                for (int i = 0; i < map.Count; i++)
                {
                    var v = map[i];
                    if (cond == null || cond(v))
                    {
                        return v;
                    }
                }
                return default;
            }
            finally
            {
                ExitUpgradeableReadLock();
            }
        }

        public V[] All(Predicate<V> cond = null)
        {
            EnterUpgradeableReadLock();
            try
            {
                if (map == null || (Environment.TickCount & int.MaxValue) >= expiry) // whether to load
                {
                    EnterWriteLock();
                    try
                    {
                        map = loader();
                    }
                    catch (Exception ex)
                    {
                        excep = ex;
                    }
                    finally
                    {
                        expiry = (Environment.TickCount & int.MaxValue) + age * 1000;
                        ExitWriteLock();
                    }
                }
                // search
                List<V> lst = null;
                for (int i = 0; i < map.Count; i++)
                {
                    var v = map[i];
                    if (cond == null || cond(v))
                    {
                        if (lst == null) lst = new List<V>(16);
                        lst.Add(v);
                    }
                }
                return lst?.ToArray();
            }
            finally
            {
                ExitUpgradeableReadLock();
            }
        }
    }
}