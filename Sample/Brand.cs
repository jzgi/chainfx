using Greatbone.Core;

namespace Greatbone.Sample
{
	public class Brand
	{
		public string Id;

		public string Name;

		public char[] Credential { get; set; }

		public long ModifiedOn { get; set; }

		public string Key => Id;

		public void From(IInput r)
		{
		}

		public void To(IOutput w)
		{
		}
	}
}