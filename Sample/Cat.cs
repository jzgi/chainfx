using Greatbone.Core;
using static Greatbone.Core.XUtility;

namespace Greatbone.Sample
{
    ///
    public class Cat : IPersist
    {

        internal int id;
        internal string title;
        internal byte[] img;
        internal string filter;
        internal bool disabled;

        public void Load(ISource s, byte x = 0)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(title), ref title);
            if (x.Ya(BIN)) { s.Get(nameof(img), ref img); }
            s.Get(nameof(filter), ref filter);
            s.Get(nameof(disabled), ref disabled);
        }

        public void Dump<R>(ISink<R> s, byte x = 0) where R : ISink<R>
        {
            if (x.Ya(AUTO)) s.Put(nameof(id), id);
            s.Put(nameof(title), title);
            if (x.Ya(BIN)) { s.Put(nameof(img), img); }
            s.Put(nameof(filter), filter);
            s.Put(nameof(disabled), disabled);
        }

    }

}