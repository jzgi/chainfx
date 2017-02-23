using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Greatbone.Core
{
    /// 
    /// A response cache for polling of events in a service.
    /// 
    public class EventQueue : IRollable
    {
        // keyed by target uri
        readonly ConcurrentDictionary<string, Que> queues;

        Action<int>[] handlers;

        internal EventQueue(int concurrency, int capcity)
        {
            queues = new ConcurrentDictionary<string, Que>(concurrency, capcity);
            handlers = new Action<int>[8];
        }

        public void SetHandler(int ordinal, Action<int> a)
        {
            handlers[ordinal] = a;
        }

        public void Add(string target, int seconds, IContent content)
        {
            Que e = new Que(seconds, content, Environment.TickCount);
            queues.AddOrUpdate(target, e, (k, v) => e.Merge(v));
        }

        public string Name=>null;

        public void Remove(string target)
        {
            return;
        }

        public void Clear()
        {
            return;
        }

        internal void Clean()
        {
            int now = Environment.TickCount;

            // a single loop to clean up expired items
            using (var enm = queues.GetEnumerator())
            {
                while (enm.MoveNext())
                {
                    Que e = enm.Current.Value;
                    if (e.IfExpired(now))
                    {
                        Que old;
                        queues.TryRemove(enm.Current.Key, out old);
                    }
                }
            }
        }

        internal bool TryGetContent(string target, out IContent v)
        {
            Que e;
            if (queues.TryGetValue(target, out e))
            {
                e.Increment();
                v = e.content;
                return true;
            }
            v = null;
            return false;
        }

        class Que
        {
            // ticks of expiration
            int expiry;

            // can be null
            internal IContent content;

            int ticks;

            int hits;

            internal Que(int expiry, IContent content, int ticks)
            {
                this.expiry = expiry;
                this.content = content;
                this.ticks = ticks;
            }

            internal void Increment()
            {
                Interlocked.Increment(ref hits);
            }

            internal int Counter => hits;

            internal bool IfExpired(int now)
            {
                return (ticks + expiry * 60000) < now;
            }

            internal Que Merge(Que e)
            {
                expiry = e.expiry;
                hits += e.hits;
                content = e.content;
                return this;
            }
        }
    }
}