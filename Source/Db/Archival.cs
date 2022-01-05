namespace SkyChain.Db
{
    /// <summary>
    /// An abstract archival entry.
    /// </summary>
    public class Archival : IData
    {
        internal int seq_;

        internal string cs_;

        internal string blockcs_;

        // for remote transaction
        //

        internal short peer_;

        internal int acct_;

        public virtual void Read(ISource s, short proj = 0x0fff)
        {
            s.Get(nameof(seq_), ref seq_);
            s.Get(nameof(cs_), ref cs_);
            s.Get(nameof(blockcs_), ref blockcs_);
            s.Get(nameof(peer_), ref peer_);
            s.Get(nameof(acct_), ref acct_);
        }

        public virtual void Write(ISink s, short proj = 0x0fff)
        {
            s.Put(nameof(seq_), seq_);
            s.Put(nameof(cs_), cs_);
            s.Put(nameof(blockcs_), blockcs_);
            s.Put(nameof(peer_), peer_);
            s.Put(nameof(acct_), acct_);
        }

        public int Seq => seq_;

        public string Cs => cs_;

        public string BlockCs => blockcs_;

        public short Peer => peer_;

        public int Acct => acct_;
    }
}