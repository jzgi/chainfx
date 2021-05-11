namespace SkyChain.Db
{
    /// <summary>
    /// A queued transaction record before being archived.
    /// </summary>
    public class Que : _State, IKeyable<int>
    {
        public new static readonly Que Empty = new Que();

        internal int id;

        public override void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(id), ref id);

            base.Read(s, proj);
        }

        public override void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(id), id);

            base.Write(s, proj);
        }

        public int Key => id;
    }
}