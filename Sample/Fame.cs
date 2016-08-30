using Greatbone.Core;

namespace Greatbone.Sample
{
	public class Fame : ISerial
	{
		public long ModifiedOn { get; set; }

		public string Key { get; }

		public void ReadFrom(IReader r)
		{
			throw new System.NotImplementedException();
		}

		public void WriteTo(IWriter w)
		{
			throw new System.NotImplementedException();
		}
	}
}