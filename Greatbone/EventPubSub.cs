namespace Greatbone
{
    /// <summary>
    /// The publishing of data events.
    /// </summary>
    public struct EventPub
    {
        readonly string flow;

        readonly string sql;

        internal EventPub(string flow)
        {
            this.flow = flow;
            this.sql = "SELECT * FROM \"" + flow + "\" WHERE pub_id > @1 LIMIT @2";
        }

        public string Flow => flow;

        public string Sql => sql;
    }
    
    /// <summary>
    /// The subscribingof a particular type of data events.
    /// </summary>
    public struct EventSub
    {
        readonly string peer;

        readonly string flow;

        readonly EventHandler handler;

        public EventSub(string peer, string flow, EventHandler handler)
        {
            this.peer = peer;
            this.flow = flow;
            this.handler = handler;
        }
    }
}