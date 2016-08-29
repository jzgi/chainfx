using Greatbone.Core;

namespace Greatbone.Sample
{
	public struct Msg : ISerial
	{
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