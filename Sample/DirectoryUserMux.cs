using Greatbone.Core;

namespace Greatbone.Sample
{
    public class DirectoryUserMux : WebMux<User>
    {
        public DirectoryUserMux(WebHub hub) : base(hub)
        {
        }

        ///
        /// Gets a token
        ///
        public override void Default(WebContext wc, User zone)
        {
//            wc.Response.SendFileAsync()
        }

        ///
        /// The user modifies this account
        ///
        public void Modify(WebContext wc, User zone)
        {
//            wc.Response.SendFileAsync()
        }

        ///
        /// The user drops this account
        ///
        public void Drop(WebContext wc, User zone)
        {
//            wc.Response.SendFileAsync()
        }
    }
}