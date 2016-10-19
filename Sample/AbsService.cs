using Greatbone.Core;

namespace Greatbone.Sample
{

    ///
    /// <summary>
    /// The common base class for all service controllers.
    /// </summary>
    ///
    public abstract class AbsService : WebService
    {
        public AbsService(WebConfig cfg) : base(cfg)
        {
        }

        protected override bool Authenticate(WebContext wc)
        {
            string h = wc.Header("Authorization");
            if (h != null)
            {
                string v = (string)h;
                v.StartsWith("Bearer "); // Bearer scheme
                JTextParse parse = new JTextParse(v.Substring(7));
                JObj jo = (JObj)parse.Parse();
                Token tok = new Token();
                tok.Load(jo);
                wc.Token = tok;

                return true;
            }
            return false;
        }
    }
}