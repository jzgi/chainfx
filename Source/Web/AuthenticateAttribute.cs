using System;
using System.Threading.Tasks;
using static SkyChain.CryptoUtility;

namespace SkyChain.Web
{
    /// <summary>
    /// To determine principal identity based on current web context. The interaction with user, however, is not included.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
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


        public static string EncryptPrincipal<P>(P prin, byte proj) where P : IData
        {
            var cnt = new JsonContent(true, 4096);
            try
            {
                cnt.PutToken(prin, proj); // use the special putting method to append time stamp
                var buf = cnt.Buffer;
                int count = cnt.Count;

                Encrypt(buf, count, Application.PrivateKey);
                return BytesToHex(buf, count);
            }
            finally
            {
                cnt.Clear();
            }
        }

        public static P DecryptPrincipal<P>(string token) where P : IData, new()
        {
            var bytes = HexToBytes(token);
            Decrypt(bytes, bytes.Length, Application.PrivateKey);
            // deserialize
            try
            {
                var jo = (JObj) new JsonParser(bytes, bytes.Length).Parse();
                // check time expiry
                DateTime stamp = jo["$"];
                if ((DateTime.Now - stamp).Hours > 2)
                {
                    return default;
                }

                // construct a principal object
                P prin = new P();
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