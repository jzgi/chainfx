namespace Greatbone.Core
{
	///
	/// Rerepsents an object that can be converted from/to JSON, BSON or XML document representations.
	///
	public interface IDoc
	{
		void From(IDocReader r);

		void To(IDocWriter w);
	}
}