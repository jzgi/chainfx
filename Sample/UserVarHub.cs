using System.Net;
using System.Text;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// <summary>The features about a particular user principal.</summary>
    /// /-/
    ///
    public class UserVarHub : WebVarHub
    {
        public UserVarHub(WebBuild bld) : base(bld)
        {
        }

        ///
        /// Gets a token
        ///
        public override void @default(WebContext wc, string id)
        {
            string password = null;
            if (wc.Get("password", ref password))
            {
                wc.Response.StatusCode = 400;
                return;
            }
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT id, credential, name FROM users WHERE id = @id", (p) => p.Put("@id", id)))
                {
                    User o = new User();
                    dc.Got(ref o.id);
                    dc.Got(ref o.credential);
                    dc.Got(ref o.name);

                    string md5 = ComputeMD5(password);
                    if (md5.Equals(o.credential))
                    {
                        wc.StatusCode = 200;
                        // wc.SetSerialObj(o);
                    }
                    else
                    {
                        wc.Response.StatusCode = 400;
                    }
                }
                else
                {
                    wc.Response.StatusCode = 404;
                }
            }
        }

        /// <summary>To modify the user's profile, normally by him/her self.</summary>
        ///
        [IfSelf]
        public void ChPwd(WebContext wc, string userid)
        {
            Obj r = (Obj)wc.Data;

            int ret = 0;

            using (var dc = Service.NewDbContext())
            {
                if (dc.Execute("UPDATE users SET password = @id WHERE id = @id", (p) => p.Put("@id", userid)) > 0)
                {
                    wc.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    wc.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                }
            }
            //            wc.Response.SendFileAsync()
        }

        ///
        /// The user drops this account
        ///
        public void Drop(WebContext wc, string x)
        {
            //            wc.Response.SendFileAsync()
        }

        internal static string ComputeMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}