using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// <summary>The user directory service</summary>
    ///
    public class UserService : WebService
    {
        public UserService(WebServiceBuilder wsc) : base(wsc)
        {
            SetXHub<UserXHub>(false);
        }

        ///
        /// Registers or creates a user account.
        ///
        public void Register(WebContext wc)
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

        public void Search(WebContext wc)
        {
            ArraySegment<byte> bytes = wc.Request.ByteArray();
            ad = wc;
        }

        WebContext ad;
    }
}