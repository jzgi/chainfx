namespace Chainly.Web
{
    /// <summary>
    /// An entry of cached web resource, that might be emptied 
    /// </summary>
    public class WebCacheEntry : IContent
    {
        // content type
        string ctype;

        // fixed content byte array
        byte[] array;

        // etag
        string etag;


        internal WebCacheEntry(IContent c)
        {
            ctype = c.CType;
            if (c is DynamicContent dyn)
            {
                array = dyn.ToByteArray();
            }
            else
            {
                array = c.Buffer;
            }
            etag = c.ETag;
        }

        public string CType => ctype;

        public byte[] Buffer => array;

        public int Count => array.Length;

        public string ETag => etag;

        /// <summary>
        /// cacheable response status, 0 means emptied
        /// </summary>
        public int Code { get; internal set; }

        /// time ticks when entered or emptied
        public int Tick { get; internal set; }

        /// <summary>
        /// maxage in seconds
        /// </summary>
        public int MaxAge { get; internal set; }

        public bool IsEmptied => Code == 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nowtick"></param>
        /// <returns>false to indicate a removal of the entry</returns>
        internal bool TryClean(int nowtick)
        {
            lock (this)
            {
                int pass = nowtick - (Tick + MaxAge * 1000);

                if (Code == 0) return pass < 900 * 1000; // 15 minutes

                if (pass >= 0) // to clear this reply
                {
                    Code = 0; // set to cleared
                    // note: buffer won't return to pool
                    array = null;
                    MaxAge = 0;
                    Tick = nowtick; // time being cleared
                }

                return true;
            }
        }

        internal bool TryGiveBy(WebContext wc, int now)
        {
            lock (this)
            {
                if (Code == 0)
                {
                    return false;
                }

                var remain = (short) (((Tick + MaxAge * 1000) - now) / 1000); // remaining in seconds
                if (remain > 0)
                {
                    short age = (short) ((now - Tick) / 1000); // age in seconds
                    wc.SetHeader("Age", age);
                    // set for response
                    wc.Give(Code, this, true, MaxAge);
                    return true;
                }

                return false;
            }
        }
    }
}