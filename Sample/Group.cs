using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    public class Group : IPersist
    {
        internal int id;

        internal string title;

        internal short subtype;

        internal bool disabled;

        public void Load(ISource sc, ushort x = 0xffff)
        {
            sc.Got(nameof(id), ref id);
            sc.Got(nameof(title), ref title);
            sc.Got(nameof(subtype), ref subtype);
            sc.Got(nameof(disabled), ref disabled);
        }

        public void Save<R>(ISink<R> sk, ushort x = 0xffff) where R : ISink<R>
        {
            sk.Put(nameof(id), id);
            sk.Put(nameof(title), title);
            sk.Put(nameof(subtype), subtype);
            sk.Put(nameof(disabled), disabled);
        }
    }

}