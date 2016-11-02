using Greatbone.Core;

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

        public void Load(ISource s, byte x = 0xff)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(title), ref title);
            if (x.On(XUtility.BIN))
            {
                s.Get(nameof(img), ref img);
            }
            s.Get(nameof(filter), ref filter);
            s.Get(nameof(disabled), ref disabled);
        }

        public void Dump<R>(ISink<R> s, byte x = 0xff) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(title), title);
            if (x.On(XUtility.BIN))
            {
                s.Put(nameof(img), img);
            }
            s.Put(nameof(filter), filter);
            s.Put(nameof(disabled), disabled);
        }

    }

}