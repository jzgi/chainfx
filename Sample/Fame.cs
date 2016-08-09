using Greatbone.Core;

namespace Greatbone.Sample
{
	public class Fame : IZone
	{
		internal string key;

		internal string name;

		public char[] Credential { get; set; }

		public long ModifiedOn { get; set; }


		public string Key => key;


		public struct Address
		{
			internal string address;

			internal string postal;

		}
	}
}