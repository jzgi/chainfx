namespace Greatbone.Core
{
	/// <summary>
	/// Rerepsents an object that can be converted from/to serialized/deserialized forms, such as JSON and Binary JSON.
	/// </summary>
	public interface ISerial
	{
		void ReadFrom(ISerialReader r);

		void WriteTo(ISerialWriter w);
	}
}