namespace Greatbone.Core
{
	public delegate void EventDoer(EqcContext ec);

	public class EqcSubscribe : IMember
	{
		public string Topic { get; }

		private readonly EventDoer doer;

		internal EqcSubscribe(string topic, EventDoer doer)
		{
			// NOTE: strict method nzame as key here to avoid the default base url trap
			Topic = topic;
			this.doer = doer;
		}

		internal void Do(EqcContext ec)
		{
			doer(ec);
		}

		public string Key => Topic;

		public override string ToString()
		{
			return Topic;
		}
	}
}