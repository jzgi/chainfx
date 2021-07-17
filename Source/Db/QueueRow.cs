namespace SkyChain.Db
{
    /// <summary>
    /// A queued journal data row.
    /// </summary>
    public class QueueRow : _Row, IKeyable<int>
    {
        public new static readonly QueueRow Empty = new QueueRow();

        internal int id;

        internal short status;

        public override void Read(ISource s, byte proj = 15)
        {
            if ((proj & ID) == ID)
            {
                s.Get(nameof(id), ref id);
            }

            base.Read(s, proj);

            s.Get(nameof(status), ref status);
        }

        public override void Write(ISink s, byte proj = 15)
        {
            if ((proj & ID) == ID)
            {
                s.Put(nameof(id), id);
            }

            base.Write(s, proj);

            s.Put(nameof(status), status);
        }

        public int Key => id;
    }
}