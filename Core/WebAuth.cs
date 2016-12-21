namespace Greatbone.Core
{

    public abstract class WebAuth
    {
        // mask for token encoding/decoding
        readonly int mask;

        // order for token encoding/decoding
        readonly int order;

        /// The cookie domain to apply
        readonly string domain;

        /// The absolute or relative URL of the signon user interface.
        readonly string signon;

        public WebAuth(int mask, int order, string domain = null, string signon = "/signon")
        {
            this.mask = mask;
            this.order = order;
            this.domain = domain;
            this.signon = signon;
        }

        public string Domain => domain;

        public string SignOn => signon;

        public abstract void Authenticate(WebActionContext ac);

        public string Decode(string tokstr)
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
        static int Dv(char h)
        {
            int v = h - '0';
            if (v >= 0 && v <= 9)
            {
                return v;
            }
            else
            {
                v = h - 'a';
                if (v >= 0 && v <= 5) return 10 + v;
            }
            return 0;
        }
    }

    ///
    /// An authenticator for web service(s)
    ///
    public class WebAuth<TH, TC> : WebAuth where TH : IToken, new() where TC : IToken, new()
    {
        public WebAuth(int mask, int order, string domain = null, string signon = "/signon") : base(mask, order, domain, signon)
        {
        }

        public override void Authenticate(WebActionContext ac)
        {
            string tokstr = null;
            string hv = ac.Header("Authorization");
            if (hv != null && hv.StartsWith("Bearer ")) // the Bearer scheme
            {
                tokstr = hv.Substring(7);
                try
                {
                    string jsonstr = Decode(tokstr);
                    ac.Principal = JsonUtility.StringToData<TH>(jsonstr);
                }
                catch { }
            }
            else if (ac.Cookies.TryGetValue("Bearer", out tokstr))
            {
                try
                {
                    string jsonstr = Decode(tokstr);
                    ac.Principal = JsonUtility.StringToData<TC>(tokstr);
                }
                catch { }
            }
        }
    }
}