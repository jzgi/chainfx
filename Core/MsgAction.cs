namespace Greatbone.Core
{
    public delegate void MsgDoer(MsgContext mc);

    public class MsgAction : IKeyed
    {
        public string Topic { get; }

        public string Filter { get; }

        readonly MsgDoer doer;

        internal MsgAction(string topic, string filter, MsgDoer doer)
        {
            // NOTE: strict method nzame as key here to avoid the default base url trap
            Topic = topic;
            Filter = filter;
            this.doer = doer;
        }

        internal void Do(MsgContext me)
        {
            doer(me);
        }

        public string Key => Topic;

        public override string ToString()
        {
            return Topic;
        }
    }
}