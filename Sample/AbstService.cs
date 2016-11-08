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
        readonly Login[] logins;

        public AbstService(WebConfig cfg) : base(cfg)
        {
            logins = JUtility.FileToArr<Login>(cfg.GetFilePath("$realm.json"));
        }


        protected override IPrincipal GetPrincipal(bool token, string idstr)
        {
            if (token) // token
            {
                string plain = StrUtility.Decrypt(idstr, 0x4a78be76, 0x1f0335e2); // plain token
                JParse par = new JParse(plain);
                try
                {
                    JObj jo = (JObj)par.Parse();
                    return jo.ToObj<Token>();
                }
                catch { }
            }
            else // username
            {
                if (logins != null)
                {
                    for (int i = 0; i < logins.Length; i++)
                    {
                        Login lgn = logins[i];
                        if (lgn.id.Equals(idstr)) return lgn;
                    }
                }
            }
            return null;
        }

        [CheckAdmin]
        public virtual void mgmt(WebContext wc, string subscpt)
        {
            if (Children != null)
            {
                wc.SendMajorLayout(200, "模块管理", a =>
                {
                    for (int i = 0; i < Children.Count; i++)
                    {
                        WebWork child = Children[i];
                        AbstModule mdl = child as AbstModule;
                        if (mdl == null) continue;

                        a.T("<li><a href=\"").T(mdl.Key).T("/mgmt\">").T(mdl.Key).T("</a></li>");
                    }
                },
                true);
            }
        }

    }

}