namespace SkyChain.Db
{
    /// <summary>
    /// An archived record in a block.
    /// </summary>
    public class Archival : _State, IKeyable<long>
    {
        public new static readonly Archival Empty = new Archival();

        public const byte PACKING = 0x10;

        internal decimal bal;

        internal short peerid;

        internal long seq;

        internal long cs;

        internal long blockcs;

        public override void Read(ISource s, byte proj = 15)
        {
            base.Read(s, proj);

            if ((proj & PACKING) == PACKING)
            {
                s.Get(nameof(bal), ref bal);
                s.Get(nameof(peerid), ref peerid);
                s.Get(nameof(seq), ref seq);
                s.Get(nameof(cs), ref cs);
                s.Get(nameof(blockcs), ref blockcs);
            }
        }

        public override void Write(ISink s, byte proj = 15)
        {
            base.Write(s, proj);

            if ((proj & PACKING) == PACKING)
            {
                s.Put(nameof(bal), bal);
                s.Put(nameof(peerid), peerid);
                s.Put(nameof(seq), seq);
                s.Put(nameof(cs), cs);
                s.Put(nameof(blockcs), blockcs);
            }
        }

        public long Key => seq;

        public decimal Bal => bal;
    }
}