using System;
using System.Threading.Tasks;

namespace ChainBase.Web
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


        public static string Encrypt<P>(P prin, byte proj) where P : IData
        {
            var cnt = new JsonContent(4096);
            try
            {
                cnt.Put(null, prin, proj);
                var bytebuf = cnt.Buffer;
                int count = cnt.Count;

                int mask = Framework.cipher;
                int[] masks = {(mask >> 24) & 0xff, (mask >> 16) & 0xff, (mask >> 8) & 0xff, mask & 0xff};
                var charbuf = new char[count * 2]; // the target
                int p = 0;
                for (int i = 0; i < count; i++)
                {
                    // masking
                    int b = bytebuf[i] ^ masks[i % 4];

                    //transform
                    charbuf[p++] = HEX[(b >> 4) & 0x0f];
                    charbuf[p++] = HEX[(b) & 0x0f];

                    // reordering
                }

                return new string(charbuf, 0, charbuf.Length);
            }
            finally
            {
                BufferUtility.Return(cnt.Buffer);
            }
        }

        public static P Decrypt<P>(string token) where P : IData, new()
        {
            int mask = Framework.cipher;
            int[] masks = {(mask >> 24) & 0xff, (mask >> 16) & 0xff, (mask >> 8) & 0xff, mask & 0xff};
            int len = token.Length / 2;
            var str = new Text(1024);
            int p = 0;
            for (int i = 0; i < len; i++)
            {
                // TODO reordering

                // transform to byte
                int b = (byte) (Dv(token[p++]) << 4 | Dv(token[p++]));
                // masking
                str.Accept((byte) (b ^ masks[i % 4]));
            }

            // deserialize
            try
            {
                var jo = (JObj) new JsonParser(str.ToString()).Parse();
                P prin = new P();
                prin.Read(jo, 0xff);
                return prin;
            }
            catch
            {
                return default;
            }
        }


        // hexidecimal characters
        static readonly char[] HEX = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        // return digit value
        static int Dv(char hex)
        {
            int v = hex - '0';
            if (v >= 0 && v <= 9)
            {
                return v;
            }

            v = hex - 'A';
            if (v >= 0 && v <= 5) return 10 + v;
            return 0;
        }
    }
}