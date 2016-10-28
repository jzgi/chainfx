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
            if (h == null) return false;
            string v = (string)h;
            if (v.StartsWith("Bearer ")) // the Bearer scheme
            {
                string tokstr = v.Substring(7);
                string plain = StrUtility.Decrypt(tokstr, 0x4a78be76, 0x1f0335e2);
                JTextParse jtp = new JTextParse(plain);
                try
                {
                    JObj jo = (JObj)jtp.Parse();
                    wc.Principal = jo.ToObj<Token>();
                    return true;
                }
                catch
                {
                }
            }
            else if (v.StartsWith("Digest ")) // the Digest scheme
            {
                FieldParse fp = new FieldParse(v);
                string username = fp.Parameter("username");
                string realm = fp.Parameter("realm");
                string nonce = fp.Parameter("nonce");
                string uri = fp.Parameter("uri");
                string response = fp.Parameter("response");

                // find prin
                IPrincipal login = GetLogin(username);
                if (login != null)
                {
                    string H_A2 = StrUtility.MD5(wc.Method + ':' + uri); // A2 = Method ":" digest-uri-value
                    string request_digest = StrUtility.MD5(login.Credential + ':' + nonce + ':' + H_A2); // request-digest = KD ( H(A1), unq(nonce-value) ":" H(A2) ) >

                    if (request_digest.Equals(response)) // matched
                    {
                        // success
                        wc.Principal = login;
                        return true;
                    }
                }
            }
            return false;
        }

        protected virtual IPrincipal GetLogin(string username)
        {
            return null;
        }

    }
}