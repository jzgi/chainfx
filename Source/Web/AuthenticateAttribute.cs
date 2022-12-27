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
            var bdr = new JsonBuilder(true, 4096);
            try
            {
                var secret = Application.Secret;
                bdr.PutToken(prin, msk); // use the special putting method to append time stamp

                // + ':' + secrent
                bdr.Add(':');
                bdr.Add(secret);

                // create and add signature
                using var md5 = MD5.Create();
                var sig = md5.ComputeHash(bdr.Buffer, 0, bdr.Count); // 16 bytes
                bdr.RemoveBytes(secret.Length); // take out secret and add signature
                bdr.AddBytes(sig);

                // convert to base64 and return
                return Convert.ToBase64String(bdr.Buffer, 0, bdr.Count);
            }
            finally
            {
                bdr.Clear();
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