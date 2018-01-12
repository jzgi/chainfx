using System;

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

        public CacheMap(Action<CacheMap<K, V>> loader, int age = 3600 * 12)
        {
            this.age = age;
            this.loader = loader;
        }

        public int Age => age;

        public Exception Excep => excep;

        void Reload()
        {
            if (Environment.TickCount >= expiry)
            {
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
                }
            }
        }

        public override V this[K key]
        {
            get
            {
                lock (this)
                {
                    Reload();
                    return base[key];
                }
            }
        }

        public override V First(Predicate<V> cond = null)
        {
            lock (this)
            {
                Reload();
                return base.First(cond);
            }
        }

        public override V[] All(Predicate<V> cond = null)
        {
            lock (this)
            {
                Reload();
                return base.All(cond);
            }
        }

        public override void ForEach(Func<K, V, bool> cond, Action<K, V> hand)
        {
            lock (this)
            {
                Reload();
                base.ForEach(cond, hand);
            }
        }
    }
}