using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// <summary>The user directory service</summary>
    ///
    public class UserModule : WebModule
    {
        public UserModule(WebConfig cfg) : base(cfg)
        {
            SetVarHub<UserVarHub>(false);
        }

        ///
        /// Creates a new user account.
        ///
        public void New(WebContext wc)
        {
            User u = null;

            using (var sc = Service.NewSqlContext())
            {
                sc.Execute("INSERT INTO users () VALUES (@id, @credential, @name, @fame, @brand, @loggedin)",
                    p =>
                    {
                        p.Set("@id", "");
                    }
                );

                wc.Response.SetContent(u);
            }
        }

        public void Find(WebContext wc)
        {
            ArraySegment<byte> bytes = wc.Request.Bytes;
            ad = wc;
        }

        WebContext ad;
    }
}