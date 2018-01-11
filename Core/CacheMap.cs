using System;
using System.Threading;

namespace Greatbone.Core
{
    /// <summary>
    /// A caching key/value dataset that periodically reloads.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class CacheMap<K, V> : Map<K, V>
    {
        readonly Action<CacheMap<K, V>> loader;

        Exception excep;

        readonly int age; //in seconds

        // tick count,   
        int expiry;

        readonly ReaderWriterLockSlim lck = new ReaderWriterLockSlim();

        public CacheMap(Action<CacheMap<K, V>> loader, int age = 3600 * 12)
        {
            this.age = age;
            this.loader = loader;
        }

        public int Age => age;

        public Exception Excep => excep;

        public override void Begin()
        {
            lck.EnterUpgradeableReadLock();

            if (Environment.TickCount >= expiry)
            {
                lck.EnterWriteLock();
                try
                {
                    Clear();
                    loader(this);
                }
                catch (Exception ex)
                {
                    excep = ex;
                }
                finally
                {
                    expiry = (Environment.TickCount & int.MaxValue) + age * 1000;
                    lck.ExitWriteLock();
                }
            }
        }

        public override void End()
        {
            lck.ExitUpgradeableReadLock();
        }

        public override V this[K key]
        {
            get
            {
                Begin();
                try
                {
                    return base[key];
                }
                finally
                {
                    End();
                }
            }
        }

        public override V First(Predicate<V> cond = null)
        {
            Begin();
            try
            {
                return base.First(cond);
            }
            finally
            {
                End();
            }
        }

        public override V[] All(Predicate<V> cond = null)
        {
            Begin();
            try
            {
                return base.All(cond);
            }
            finally
            {
                End();
            }
        }

        public override void ForEach(Func<K, V, bool> cond, Action<K, V> hand, bool write = false)
        {
            Begin();
            try
            {
                if (write)
                {
                    lck.EnterWriteLock();
                    try
                    {
                        base.ForEach(cond, hand, true);
                    }
                    finally
                    {
                        lck.ExitWriteLock();
                    }
                }
                else
                {
                    base.ForEach(cond, hand, false);
                }
            }
            finally
            {
                End();
            }
        }
    }
}