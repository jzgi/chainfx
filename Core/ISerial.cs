namespace Greatbone.Core
{
	/// <summary>
	/// Rerepsents an object that can be converted from/to serialized/deserialized forms, such as JSON and Binary JSON.
	/// </summary>
	public interface ISerial
	{
		void From(ISerialReader r);

		void To(ISerialWriter w);
	}
}