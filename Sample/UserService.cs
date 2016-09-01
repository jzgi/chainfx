using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// <summary>The user directory service</summary>
	///
	public class UserService : WebService
	{
		public UserService(WebServiceContext wsc) : base(wsc)
		{
			AttachXHub<UserXHub>(false);
		}

		///
		/// Registers or creates a user account.
		///
		public void Register(WebContext wc)
		{
			string s = wc.Request.Host.Value.ToString();
		}

		public void Search(WebContext wc)
		{
		}
	}
}