using Greatbone.Core;

namespace Greatbone.Sample
{
	public class Fame : ISerial, IUnit
	{
		public long ModifiedOn { get; set; }

		public string Key { get; }

		public void From(IReader r)
		{
			throw new System.NotImplementedException();
		}

		public void To(IWriter w)
		{
			throw new System.NotImplementedException();
		}
	}
}