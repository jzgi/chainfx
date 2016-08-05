using Greatbone.Core;

namespace Greatbone.Sample
{
	public class Brand : IData, IZone
	{
		public void From(IDataInput i, int flags)
		{
			throw new System.NotImplementedException();
		}

		public void To(IDataOutput o, int flags)
		{
			throw new System.NotImplementedException();
		}

		public long ModifiedOn { get; set; }
		public string Key { get; }
	}
}