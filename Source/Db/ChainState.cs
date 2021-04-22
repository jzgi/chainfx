namespace SkyChain.Db
{
    /// <summary>
    /// An archival state in a block.
    /// </summary>
    public class ChainState : _State, IKeyable<long>
    {
        public new static readonly ChainState Empty = new ChainState();

        public const byte PACKING = 0x10;

        internal short pid;
        internal long seq;
        internal decimal bal;
        internal long cs;
        internal long blockcs;

        public override void Read(ISource s, byte proj = 15)
        {
            base.Read(s, proj);

            if ((proj & PACKING) == PACKING)
            {
                s.Get(nameof(pid), ref pid);
                s.Get(nameof(seq), ref seq);
                s.Get(nameof(bal), ref bal);
                s.Get(nameof(cs), ref cs);
                s.Get(nameof(blockcs), ref blockcs);
            }
        }

        public override void Write(ISink s, byte proj = 15)
        {
            base.Write(s, proj);

            if ((proj & PACKING) == PACKING)
            {
                s.Put(nameof(pid), pid);
                s.Put(nameof(seq), seq);
                s.Put(nameof(bal), bal);
                s.Put(nameof(cs), cs);
                s.Put(nameof(blockcs), blockcs);
            }
        }

        public long Key => seq;

        public decimal Bal => bal;
    }
}