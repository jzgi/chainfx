using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// <summary></summary>
	/// /-/
	///
    public class UserXHub : WebXHub
    {
        public UserXHub(WebBuilder builder) : base(builder)
        {
        }

        ///
        /// Gets a token
        ///
        public override void Default(WebContext wc, string unit)
        {
//            wc.Response.SendFileAsync()
        }

        /// <summary>To modify the user's profile, normally by him/her self.</summary>
        ///
        public void Modify(WebContext wc, User unit)
        {
//            wc.Response.SendFileAsync()
        }

        ///
        /// The user drops this account
        ///
        public void Drop(WebContext wc, User unit)
        {
//            wc.Response.SendFileAsync()
        }
    }
}