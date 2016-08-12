namespace Greatbone.Core
{
	public class MsgPublish : IMember
	{
		public string Topic { get; }

		public bool Subtype { get; }

		internal MsgPublish(string topic, bool subtype)
		{
			Topic = topic;
			Subtype = subtype;
		}

		public string Key => Topic;
	}
}