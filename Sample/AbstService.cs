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
            logins = JUtility.FileToArr<Login>(cfg.key + "-access.json");
        }

        [ToAdmin]
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


        protected override IPrincipal GetPrincipal(string scheme, string ident)
        {
            if ("Bearer".Equals(scheme))
            {
                JTextParse jtp = new JTextParse(ident);
                try
                {
                    JObj jo = (JObj)jtp.Parse();
                    return jo.ToObj<Token>();
                }
                catch
                {
                }
            }
            else if ("Digest".Equals(scheme))
            {
                if (logins != null)
                {
                    for (int i = 0; i < logins.Length; i++)
                    {
                        Login lgn = logins[i];
                        if (lgn.id.Equals(ident)) return lgn;
                    }
                }
            }
            return null;
        }

    }
}