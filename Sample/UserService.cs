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
            ArraySegment<byte> bytes = wc.Request.ByteArray();

            User u = wc.Request.Object<User>();
            string s = wc.Request.Host.Value.ToString();

            wc.Response.SetJson(u);
        }

        public void Search(WebContext wc)
        {
            ArraySegment<byte> bytes = wc.Request.ByteArray();
            ad = wc;
        }

        WebContext ad;
    }
}