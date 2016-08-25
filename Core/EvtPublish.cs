namespace Greatbone.Core
{
	public class EvtPublish : IMember
	{
		public string Topic { get; }

		public bool Subtype { get; }

		internal EvtPublish(string topic, bool subtype)
		{
			Topic = topic;
			Subtype = subtype;
		}

		public string Key => Topic;
	}
}