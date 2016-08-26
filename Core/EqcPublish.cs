namespace Greatbone.Core
{
	public class EqcPublish : IMember
	{
		public string Topic { get; }

		public bool Subtype { get; }

		internal EqcPublish(string topic, bool subtype)
		{
			Topic = topic;
			Subtype = subtype;
		}

		public string Key => Topic;
	}
}