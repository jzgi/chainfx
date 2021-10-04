namespace SkyChain.Db
{
    /// <summary>
    /// An abstract archival entry.
    /// </summary>
    public class _Arch : IData
    {
        public static readonly _Arch Empty = new _Arch();

        public const byte ID = 1, LATER = 2;

        // globally unique transaction number that can identify the source peer
        internal long txn;

        internal int seq;

        internal long cs;

        internal long blockcs;

        public virtual void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(txn), ref txn);
            s.Get(nameof(seq), ref seq);
            s.Get(nameof(cs), ref cs);
            s.Get(nameof(blockcs), ref blockcs);
        }

        public virtual void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(txn), txn);
            s.Put(nameof(seq), seq);
            s.Put(nameof(cs), cs);
            s.Put(nameof(blockcs), blockcs);
        }

        public long Txn => txn;

        public long Cs => cs;

        public long Blockcs => blockcs;
    }
}