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
                JTextParse parse = new JTextParse(plain);
                try
                {
                    JObj jo = (JObj)parse.Parse();
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
                IPrincipal prin = null;

                string H_A2 = StrUtility.MD5(wc.Method + ':' + uri); // A2 = Method ":" digest-uri-value
                string request_digest = StrUtility.MD5(prin.Credential + ':' + nonce + ':' + H_A2); // request-digest = KD ( H(A1), unq(nonce-value) ":" H(A2) ) >

                if (request_digest.Equals(response)) // matched
                {
                    // successfully, attach the authenticated principal object to the connection
                    wc.Principal = prin;
                    // vote to continue processing
                    return true;
                }
            }

            return false;
        }


        //    boolean authenticate(Connection conn) throws IOException {
        //         Principal prin = conn.principal;
        //         if (prin == null && authenticate != null) { // apply authentication only if not yet done
        //             Matcher m = authenticate.matcher(conn.path());
        //             if (m.matches()) { // apply authentication only if the path matches the preset pattern
        //                 Header hau = conn.header(HTTP.H_AUTHORIZATION);
        //                 if (hau != null && hau.match("Digest ")) { // supports only the Digest scheme
        //                     String username = hau.parameter("username");
        //                     String realm = hau.parameter("realm");
        //                     String nonce = hau.parameter("nonce");
        //                     String uri = hau.parameter("uri");
        //                     String response = hau.parameter("response");
        //                     // enforce DIGEST auth
        //                     if (REALM.equals(realm)) {
        //                         prin = find(username);
        //                         String H_A2 = HTTP.MD5(new StringBuilder().append(conn.method()).append(COLON).append(uri).toString()); // A2 = Method ":" digest-uri-value
        //                         String request_digest = HTTP.MD5(prin.credential + COLON + nonce + COLON + H_A2); // request-digest = KD ( H(A1), unq(nonce-value) ":" H(A2) ) >
        //                         if (request_digest.equals(response)) { // matched
        //                             // successfully, attach the authenticated principal object to the connection
        //                             conn.principal = prin;
        //                             // vote to continue processing
        //                             return true;
        //                         }
        //                     }
        //                 }
        //                 // send status
        //                 conn.sendUnauthorized(REALM);
        //                 // vote to cease processing
        //                 return false;
        //             }
        //         }
        //         // vote to continue processing
        //         return true;
        //     }

    }
}