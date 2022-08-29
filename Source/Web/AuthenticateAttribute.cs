using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ChainFx.Web
{
    /// <summary>
    /// To determine principal identity based on current web context. The interaction with user, however, is not included.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public abstract class AuthenticateAttribute : Attribute
    {
        readonly bool async;

        protected AuthenticateAttribute(bool async)
        {
            this.async = async;
        }

        public bool IsAsync => async;

        /// <summary>
        /// The synchronous version of authentication check.
        /// </summary>
        /// <remarks>The method only tries to establish principal identity within current web context, not responsible for any related user interaction.</remarks>
        /// <param name="wc">current web request/response context</param>
        /// <returns>true to indicate the web context should continue with processing; false otherwise</returns>
        public virtual bool Do(WebContext wc) => throw new NotImplementedException();

        /// <summary>
        /// The asynchronous version of authentication check.
        /// </summary>
        /// <remarks>The method only tries to establish principal identity within current web context, not responsible for any related user interaction.</remarks>
        /// <param name="wc">current web request/response context</param>
        /// <returns>true to indicate the web context should continue with processing; false otherwise</returns>
        public virtual Task<bool> DoAsync(WebContext wc) => throw new NotImplementedException();


        public static string ToToken<P>(P prin, short msk) where P : IData
        {
            var cnt = new JsonContent(true, 4096);
            try
            {
                var secret = Application.Secret;
                cnt.PutToken(prin, msk); // use the special putting method to append time stamp

                // + ':' + secrent
                cnt.Add(':');
                cnt.Add(secret);

                // create and add signature
                using var md5 = MD5.Create();
                var sig = md5.ComputeHash(cnt.Buffer, 0, cnt.Count); // 16 bytes
                cnt.RemoveBytes(secret.Length); // take out secret and add signature
                cnt.AddBytes(sig);

                // convert to base64 and return
                return Convert.ToBase64String(cnt.Buffer, 0, cnt.Count);
            }
            finally
            {
                cnt.Clear();
            }
        }

        public static P FromToken<P>(string token) where P : IData, new()
        {
            var bytes = Convert.FromBase64String(token);

            var secret = Application.Secret;

            // replace signature with secret
            Span<byte> sig = stackalloc byte[16];
            for (int i = 0; i < 16; i++)
            {
                var off = bytes.Length - 16 + i;
                sig[i] = bytes[off]; // backup 
                if (i < secret.Length)
                {
                    bytes[off] = (byte) secret[i]; // replace
                }
            }

            // compare signature
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(bytes, 0, bytes.Length - 16 + secret.Length);
            for (int i = 0; i < 16; i++)
            {
                if (sig[i] != hash[i])
                {
                    return default;
                }
            }

            // deserialize
            try
            {
                var len = bytes.Length - 16 - 1;
                var jo = (JObj) new JsonParser(bytes, len).Parse();

                // check time expiry
                DateTime stamp = jo["$"];
                if ((DateTime.Now - stamp).Hours > 2)
                {
                    return default;
                }

                // construct a principal object
                var prin = new P();
                prin.Read(jo, 0xff);
                return prin;
            }
            catch
            {
                return default;
            }
        }
    }
}