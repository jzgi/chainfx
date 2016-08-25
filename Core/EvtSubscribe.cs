namespace Greatbone.Core
{
	public delegate void EvtDoer(EvtContext ec);

	public class EvtSubscribe : IMember
	{
		public string Topic { get; }

		private readonly EvtDoer doer;

		internal EvtSubscribe(string topic, EvtDoer doer)
		{
			// NOTE: strict method nzame as key here to avoid the default base url trap
			Topic = topic;
			this.doer = doer;
		}

		internal void Do(EvtContext ec)
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