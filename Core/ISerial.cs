namespace Greatbone.Core
{
	///
	/// Rerepsents an object that can be converted from/to JSON, BSON or XML document representations.
	///
	public interface ISerial
	{
		void From(IReader r);

		void To(IWriter w);
	}
}