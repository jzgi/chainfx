using System;
using System.Text;
using System.Threading.Tasks;
using CloudUn.Web;
using static CloudUn.Framework;

namespace CloudUn.Net
{
    /// <summary>
    /// To establish principal identity. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class UserAuthenticateAttribute : AuthenticateAttribute
    {
        const string WXAUTH = "wxauth";

        public override Task<bool> DoAsync(WebContext wc) => throw new NotImplementedException();

        public UserAuthenticateAttribute() : base(false)
        {
        }

        public override bool Do(WebContext wc)
        {
            // try to restore principal from cookie token
            string token;
            if (wc.Cookies.TryGetValue(nameof(token), out token))
            {
                var o = Decrypt<User>(token);
                if (o != null)
                {
                    wc.Principal = o;
                    return true;
                }
            }

            // authenticate thru wechat or basic
            //
            User prin;
            string h_auth = wc.Header("Authorization");
            if (h_auth == null || !h_auth.StartsWith("Basic "))
            {
                return true;
            }

            // decode basic scheme
            var bytes = Convert.FromBase64String(h_auth.Substring(6));
            string orig = Encoding.ASCII.GetString(bytes);
            int colon = orig.IndexOf(':');
            string id = orig.Substring(0, colon);
            string credential = TextUtility.MD5(orig);

            // try to load principal by tel
            using var dc = NewDbContext();
            dc.Sql("SELECT ").collst(User.Empty).T(" FROM users WHERE id = @1");
            if (dc.QueryTop(p => p.Set(id)))
            {
                prin = dc.ToObject<User>();
                if (prin == null || !credential.Equals(prin.credential))
                {
                    return true;
                }
                wc.Principal = prin; // set principal for afterwrads
                wc.SetTokenCookie(prin);
            }
            return true;
        }
    }
}