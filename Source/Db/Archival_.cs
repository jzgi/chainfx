namespace SkyChain.Db
{
    /// <summary>
    /// An abstract archival entry.
    /// </summary>
    public class Archival_ : IData
    {
        public static readonly Archival_ Empty = new Archival_();

        // globally unique transaction number
        internal int seq_;
        internal long cs_;
        internal long blockcs_;

        // for remote transaction
        internal int peerid_;
        internal int coid_;

        public virtual void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(seq_), ref seq_);
            s.Get(nameof(cs_), ref cs_);
            s.Get(nameof(blockcs_), ref blockcs_);
            s.Get(nameof(peerid_), ref peerid_);
            s.Get(nameof(coid_), ref coid_);
        }

        public virtual void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(seq_), seq_);
            s.Put(nameof(cs_), cs_);
            s.Put(nameof(blockcs_), blockcs_);
            s.Put(nameof(peerid_), peerid_);
            s.Put(nameof(coid_), coid_);
        }

        public int Seq => seq_;

        public long Cs => cs_;

        public long Blockcs => blockcs_;

        public int PeerId => peerid_;
    }
}