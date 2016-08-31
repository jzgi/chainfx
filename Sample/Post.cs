using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// <summary>A brand object.</summary>
	/// <example>
	///     Brand o  new Brand(){}
	/// </example>
	public class Post : ISerial
	{
		public long ModifiedOn { get; set; }

		///
		/// <summary>Returns the key of the brand object.</summary>
		public string Key { get; }

		public void ReadFrom(ISerialReader r)
		{
			throw new System.NotImplementedException();
		}

		public void WriteTo(ISerialWriter w)
		{
			throw new System.NotImplementedException();
		}
	}
}