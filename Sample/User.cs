using Greatbone.Core;

namespace Greatbone.Sample
{
	/// <summary>A user record that is a web access token for all the services. </summary>
	public class User : IToken, ISerial
	{
		string id;

		string name;

		string mobile;

		string[] roles;

		public static string Encrypt(string orig)
		{
			return null;
		}

		public static string Decrypt(string src)
		{
			return null;
		}

		public string Login => id;

		public bool Can(string zone, int role)
		{
			return false;
		}

		public void From(IReader r)
		{
			r.Read(nameof(id), ref id);
			r.Read(nameof(name), ref name);
			r.Read(nameof(mobile), ref mobile);
			r.Read(nameof(roles), ref roles);
		}

		public void To(IWriter w)
		{
			w.Write(nameof(id), id);
			w.Write(nameof(name), name);
			w.Write(nameof(mobile), mobile);
			w.Write(nameof(roles), roles);
		}

		public string[] Roles => roles;
	}
}