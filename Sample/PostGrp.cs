using System;
using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Greatbone.Sample
{
    public struct PostGrp : IData
    {
        public static PostGrp Empty = new PostGrp();

        internal string id;
        internal string descript;
        internal DateTime creationdate;
        internal int rating;
        internal string cat;
        internal byte[] icon;

        public void Load(ISource s, byte z = 0)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(creationdate), ref creationdate);
            s.Get(nameof(descript), ref descript);
            s.Get(nameof(rating), ref rating);
            s.Get(nameof(cat), ref cat);
            if (z.Ya(BIN)) s.Get(nameof(icon), ref icon);
        }

        public void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(creationdate), creationdate);
            s.Put(nameof(descript), descript);
            s.Put(nameof(rating), rating);
            s.Put(nameof(cat), cat);
            if (z.Ya(BIN)) s.Put(nameof(icon), icon);
        }
    }
}