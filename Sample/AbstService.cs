using Greatbone.Core;

namespace Greatbone.Sample
{

    ///
    /// <summary>
    /// The common base class for all service controllers.
    /// </summary>
    ///
    public abstract class AbstService : WebService
    {
        public AbstService(WebConfig cfg) : base(cfg)
        {
        }

        [IfAdmin]
        public virtual void mgmt(WebContext wc, string subscpt)
        {
            if (Children != null)
            {
                wc.SendMajorLayout(200, "模块管理", a =>
                {
                    for (int i = 0; i < Children.Count; i++)
                    {
                        WebControl child = Children[i];
                        AbstModule mdl = child as AbstModule;
                        if (mdl == null) continue;

                        a.T("<li><a href=\"").T(mdl.Key).T("/mgmt\">").T(mdl.Key).T("</a></li>");
                    }
                },
                true);
            }
        }

        protected override bool Authenticate(WebContext wc)
        {
            string h = wc.Header("Authorization");
            if (h != null)
            {
                string v = (string)h;
                if (!v.StartsWith("Bearer ")) // Bearer scheme
                {
                    return false;
                }

                string tokstr = v.Substring(7);
                string plain = Token.Decrypt(tokstr, 12, 12);
                JTextParse parse = new JTextParse(plain);
                JObj jo = (JObj)parse.Parse();
                wc.Token = jo.ToObj<Token>();

                return true;
            }
            return false;
        }

    }
}