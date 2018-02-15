namespace Greatbone.Core
{
    public struct FlowPub
    {
        readonly string flow;

        readonly string sql;

        internal FlowPub(string flow)
        {
            this.flow = flow;
            this.sql = "SELECT * FROM \"" + flow + "\" WHERE pub_id > @1 LIMIT @2";
        }

        public string Flow => flow;

        public string Sql => sql;
    }

    public struct FlowSub
    {
        readonly string peerId;

        readonly string flow;

        readonly FlowDelegate handler;

        public FlowSub(string peerId, string flow, FlowDelegate handler)
        {
            this.peerId = peerId;
            this.flow = flow;
            this.handler = handler;
        }
    }
}