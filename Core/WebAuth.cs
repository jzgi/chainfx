namespace Greatbone.Core
{
    public abstract class WebAuth
    {
        // mask for encoding/decoding token
        readonly int mask;

        // order for encoding/decoding token
        readonly int order;

        /// The cookie domain to apply
        readonly string domain;

        /// The absolute or relative URL of the signon user interface.
        readonly string signon;

        protected WebAuth(int mask, int order, string domain = null, string signon = "/signon")
        {
            this.mask = mask;
            this.order = order;
            this.domain = domain;
            this.signon = signon;
        }

        public string Domain => domain;

        public string SignOn => signon;

        public abstract void Authenticate(WebActionContext ac);


        // hexidecimal characters
        protected static readonly char[] HEX =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };

        public string Encrypt(IToken tok)
        {
            JsonContent cont = new JsonContent(true, false, 4096);
            cont.Put(tok);
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
            return new string(charbuf, 0, charbuf.Length);
        }

        public string Decrypt(string tokstr)
        {
            int[] masks = { (mask >> 24) & 0xff, (mask >> 16) & 0xff, (mask >> 8) & 0xff, mask & 0xff };
            int len = tokstr.Length / 2;
            Str str = new Str(256);
            int p = 0;
            for (int i = 0; i < len; i++)
            {
                // reordering

                // transform to byte
                int b = (byte)(Dv(tokstr[p++]) << 4 | Dv(tokstr[p++]));

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
    }

    ///
    /// The authentication logic for web service.
    ///
    public class WebAuth<T> : WebAuth where T : IToken, new()
    {
        public WebAuth(int mask, int order, string domain = null, string signon = "/signon") : base(mask, order, domain, signon) { }

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