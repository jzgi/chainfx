namespace SkyChain.Chain
{
    /// <summary>
    /// A globally-or-locally scoped data record
    /// </summary>
    public class Replical : IData
    {
        public const byte ID = 1, LATER = 2;

        // scoped serial number
        internal int id;

        // sub-scope identifier
        internal string sub_;

        // last modified stamp
        internal long seq_;

        public virtual void Read(ISource s, byte proj = 15)
        {
            if ((proj & ID) == ID)
            {
                s.Get(nameof(id), ref id);
            }
            s.Get(nameof(sub_), ref sub_);
            s.Get(nameof(seq_), ref seq_);
        }

        public virtual void Write(ISink s, byte proj = 15)
        {
            if ((proj & ID) == ID)
            {
                s.Put(nameof(id), id);
            }
            s.Put(nameof(sub_), sub_);
            s.Put(nameof(seq_), seq_);
        }

        public long Seq => seq_;
    }
}