using System.Threading;

namespace Greatbone.Core
{
    public class Cachie
    {
        internal int status;

        // can be set to null
        internal IContent content;

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

        internal void CheckReset(int ticks)
        {
            lock (this)
            {
                if (((stamp + maxage * 1000) - ticks) / 1000 <= 0)
                {
                    status = 0;

                    if (content != null && content.Poolable)
                    {
                        BufferUtility.Return(content.ByteBuffer);
                    }
                    content = null;

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
                    ac.Cached = true;
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