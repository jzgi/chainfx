using System;

namespace SkyChain.Db
{
    /// <summary>
    /// A globally-scoped data record that can be synced to referencing db.
    /// </summary>
    public class Global_ : IData
    {
        // last modified time stamp
        internal DateTime stamp_;

        public virtual void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(stamp_), ref stamp_);
        }

        public virtual void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(stamp_), stamp_);
        }

        public DateTime Stamp => stamp_;
    }
}