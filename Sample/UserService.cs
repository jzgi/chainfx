using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// <summary>The user directory service</summary>
	///
	public class UserService : WebService
	{
		public UserService(WebBuilder builder) : base(builder)
		{
			AddSub<UserAdminSub>("admin", (x) => x.Can(null, 1));

			MountHub<UserXHub>((x) => true);
		}

		///
		/// Registers or creates a user account.
		///
		public void Register(WebContext wc)
		{
		}

		public void Search(WebContext wc)
		{
		}

	}
}