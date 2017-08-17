using System.Threading;

namespace Greatbone.Core
{
    /// <summary>
    /// An entry in the service response cache.
    /// </summary>
    public class Cachie
    {
        // response status, 0 means cleared, otherwise one of the cacheable status
        int status;

        // can be set to null
        IContent content;

        // maxage in seconds
        int maxage;

        // time ticks when entered
        int stamp;

        int hits;

        internal Cachie(int status, IContent content, int maxage, int stamp)
        {
            this.status = status;
            this.content = content;
            this.maxage = maxage;
            this.stamp = stamp;
        }

        /// <summary>
        ///  RFC 7231 cacheable status codes.
        /// </summary>
        public static bool IsCacheable(int code)
        {
            return code == 200 || code == 203 || code == 204 || code == 206 || code == 300 || code == 301 || code == 404 || code == 405 || code == 410 || code == 414 || code == 501;
        }

        internal void TryClear(int ticks)
        {
            lock (this)
            {
                if (status == 0) return;

                if (((stamp + maxage * 1000) - ticks) / 1000 <= 0)
                {
                    status = 0;
                    content = null; // NOTE: the buffer won't return to the pool
                    maxage = 0;
                    stamp = 0;
                }
            }
        }

        public int Hits => hits;

        internal bool TryGive(ActionContext ac, int ticks)
        {
            lock (this)
            {
                if (status == 0) return false;

                int remain = ((stamp + maxage * 1000) - ticks) / 1000;
                if (remain > 0)
                {
                    ac.InCache = true;
                    ac.Give(status, content, true, remain);

                    Interlocked.Increment(ref hits);

                    return true;
                }

                return false;
            }
        }

        internal Cachie MergeWith(Cachie old)
        {
            Interlocked.Add(ref hits, old.Hits);
            return this;
        }
    }
}