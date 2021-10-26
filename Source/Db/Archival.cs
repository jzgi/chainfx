namespace SkyChain.Db
{
    /// <summary>
    /// An abstract archival entry.
    /// </summary>
    public class Archival : IData
    {
        public static readonly Archival Empty = new Archival();

        // globally unique transaction number
        internal int seq_;
        internal long cs_;
        internal long blockcs_;

        // for remote transaction
        internal int peerid_;
        internal int orgid_;

        public virtual void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(seq_), ref seq_);
            s.Get(nameof(cs_), ref cs_);
            s.Get(nameof(blockcs_), ref blockcs_);
            s.Get(nameof(peerid_), ref peerid_);
            s.Get(nameof(orgid_), ref orgid_);
        }

        public virtual void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(seq_), seq_);
            s.Put(nameof(cs_), cs_);
            s.Put(nameof(blockcs_), blockcs_);
            s.Put(nameof(peerid_), peerid_);
            s.Put(nameof(orgid_), orgid_);
        }

        public int Seq => seq_;

        public long Cs => cs_;

        public long Blockcs => blockcs_;

        public int PeerId => peerid_;
    }
}