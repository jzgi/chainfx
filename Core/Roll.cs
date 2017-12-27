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

        readonly int age; //in seconds

        Map<K, V> content;

        // tick count,   
        int expiry;

        public Roll(Func<Map<K, V>> loader, int age = 3600 * 12)
        {
            this.age = age;
            this.loader = loader;
        }

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
                            content = loader();
                            expiry = (Environment.TickCount & int.MaxValue) + age * 1000;
                        }
                        finally
                        {
                            ExitWriteLock();
                        }
                    }
                    return content[key];
                }
                finally
                {
                    ExitUpgradeableReadLock();
                }
            }
        }

        public V[] FindAll(Predicate<V> cond)
        {
            EnterUpgradeableReadLock();
            try
            {
                if (content == null || (Environment.TickCount & int.MaxValue) >= expiry) // whether to load
                {
                    EnterWriteLock();
                    try
                    {
                        content = loader();
                        expiry = (Environment.TickCount & int.MaxValue) + age * 1000;
                    }
                    finally
                    {
                        ExitWriteLock();
                    }
                }
                // search
                List<V> lst = null;
                for (int i = 0; i < content.Count; i++)
                {
                    var v = content[i];
                    if (cond(v))
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