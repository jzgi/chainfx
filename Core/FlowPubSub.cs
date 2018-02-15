namespace Greatbone.Core
{
    public struct FlowPub
    {
        readonly string view;

        readonly string sql;

        internal FlowPub(string view)
        {
            this.view = view;
            this.sql = "SELECT * FROM \"" + view + "\" WHERE pubid > @1 LIMIT @2 OFFSET @3";
        }
    }

    public struct FlowSub
    {
        readonly string peer;

        readonly string pubname;

        readonly FlowDelegate handler;

        public FlowSub(string peer, string pubname, FlowDelegate handler)
        {
            this.peer = peer;
            this.pubname = pubname;
            this.handler = handler;
        }
    }
}