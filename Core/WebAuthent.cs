using System.Text;

namespace Greatbone.Core
{
    ///
    /// To authenticate user tokens
    ///
    public abstract class WebAuthent
    {
        // mask for encoding/decoding token
        readonly int mask;

        // order for encoding/decoding token
        readonly int order;

        /// The cookie max-age to apply
        readonly int maxage;

        /// The absolute or relative URL of the signon user interface.
        readonly string signon;

        protected WebAuthent(int mask, int order, int maxage = 0, string signon = "/signon")
        {
            this.mask = mask;
            this.order = order;
            this.maxage = maxage;
            this.signon = signon;
        }

        public int MaxAge => maxage;

        public string SignOn => signon;

        public abstract void Authenticate(WebActionContext ac);


        // hexidecimal characters
        protected static readonly char[] HEX =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };

        public string Encrypt(IData token)
        {
            JsonContent cont = new JsonContent(true, true, 4096); // borrow
            cont.Put(null, token);
            byte[] bytebuf = cont.ByteBuffer;
            int count = cont.Size;

            int[] masks = { (mask >> 24) & 0xff, (mask >> 16) & 0xff, (mask >> 8) & 0xff, mask & 0xff };
            char[] charbuf = new char[count * 2]; // the target 
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
            // return pool
            BufferUtility.Return(bytebuf);

            return new string(charbuf, 0, charbuf.Length);
        }

        public string Decrypt(string tokenstr)
        {
            int[] masks = { (mask >> 24) & 0xff, (mask >> 16) & 0xff, (mask >> 8) & 0xff, mask & 0xff };
            int len = tokenstr.Length / 2;
            Text str = new Text(256);
            int p = 0;
            for (int i = 0; i < len; i++)
            {
                // reordering

                // transform to byte
                int b = (byte)(Dv(tokenstr[p++]) << 4 | Dv(tokenstr[p++]));

                // masking
                str.Accept((byte)(b ^ masks[i % 4]));
            }
            return str.ToString();
        }


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

        public void SetCookieHeader(WebActionContext ac, IData token)
        {
            StringBuilder sb = new StringBuilder("Bearer=");
            string tokenstr = Encrypt(token);
            sb.Append(tokenstr);
            sb.Append("; HttpOnly");
            if (maxage != 0)
            {
                sb.Append("; Max-Age=").Append(maxage);
            }
            // detect domain from the Host header
            string host = ac.Header("Host");
            if (!string.IsNullOrEmpty(host))
            {
                // if the last part is not numeric
                int lastdot = host.LastIndexOf('.');
                if (!char.IsDigit(host[lastdot + 1])) // a domain name is given
                {
                    int dot = host.LastIndexOf('.', lastdot - 1);
                    if (dot != -1)
                    {
                        string domain = host.Substring(dot + 1);
                        sb.Append("; Domain=").Append(domain);
                    }
                }
            }
            // set header
            ac.SetHeader("Set-Cookie", sb.ToString());
        }
    }

    ///
    /// The authentication logic for web service.
    ///
    public class WebAuthent<T> : WebAuthent where T : IData, new()
    {
        public WebAuthent(int mask, int order, int maxage = 0, string signon = "/signon") : base(mask, order, maxage, signon) { }

        public override void Authenticate(WebActionContext ac)
        {
            string tokstr;
            string hv = ac.Header("Authorization");
            if (hv != null && hv.StartsWith("Bearer ")) // the Bearer scheme
            {
                tokstr = hv.Substring(7);
                string jsonstr = Decrypt(tokstr);
                ac.Token = JsonUtility.StringToObject<T>(jsonstr);
            }
            else if (ac.Cookies.TryGetValue("Bearer", out tokstr))
            {
                string jsonstr = Decrypt(tokstr);
                ac.Token = JsonUtility.StringToObject<T>(jsonstr);
            }
        }
    }
}