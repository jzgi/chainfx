using SkyChain.Web;

namespace SkyChain.Web
{
    /// <summary>
    /// An entry of cached web response, that might be cleared but not removed 
    /// </summary>
    public class WebCachie
    {
        // cacheable response status, or 0 means cleared
        int code;

        // can be set to null
        IContent content;

        private byte[] bytes;

        // maxage in seconds
        int maxage;

        // time ticks when entered or cleared
        int stamp;

        internal WebCachie(int code, IContent content, int maxage, int stamp)
        {
            this.code = code;
            this.content = content;
            this.maxage = maxage;
            this.stamp = stamp;
        }

        /// <summary>
        /// RFC 7231 cacheable status codes.
        /// </summary>
        public static bool IsCacheable(int code)
        {
            return code == 200 || code == 203 || code == 204 || code == 206 || code == 300 || code == 301 || code == 404 || code == 405 || code == 410 || code == 414 || code == 501;
        }

        public bool IsCleared => code == 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="now"></param>
        /// <returns>false to indicate a removal of the entry</returns>
        internal bool TryClean(int now)
        {
            lock (this)
            {
                int pass = now - (stamp + maxage * 1000);

                if (code == 0) return pass < 900 * 1000; // 15 minutes

                if (pass >= 0) // to clear this reply
                {
                    code = 0; // set to cleared
                    // note: buffer won't return to pool
                    content = null;
                    maxage = 0;
                    stamp = now; // time being cleared
                }

                return true;
            }
        }

        internal bool TryGive(WebContext wc, int now)
        {
            lock (this)
            {
                if (code == 0)
                {
                    return false;
                }

                short remain = (short) (((stamp + maxage * 1000) - now) / 1000); // remaining in seconds
                if (remain > 0)
                {
                    wc.IsInCache = true;
                    wc.Give(code, content, true, remain);
                    return true;
                }

                return false;
            }
        }
    }
}