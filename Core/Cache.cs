using System;
using System.Threading;

namespace Greatbone.Core
{
    public class Cache<K, V> : ReaderWriterLockSlim
    {
        readonly int age; //in seconds

        readonly Func<Map<K, V>> loader;

        Map<K, V> content;

        // time count
        int expiry;

        public Cache(int age, Func<Map<K, V>> loader)
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
    }
}