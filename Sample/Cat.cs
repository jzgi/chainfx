using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    public class Cat : IPersist
    {
        internal int id;

        internal string title;

        internal string filter;

        internal bool disabled;

        public void Load(ISource sc, uint x = 0)
        {
            sc.Got(nameof(id), ref id);
            sc.Got(nameof(title), ref title);
            sc.Got(nameof(filter), ref filter);
            sc.Got(nameof(disabled), ref disabled);
        }

        public void Save<R>(ISink<R> sk, uint x = 0) where R : ISink<R>
        {
            sk.Put(nameof(id), id);
            sk.Put(nameof(title), title);
            sk.Put(nameof(filter), filter);
            sk.Put(nameof(disabled), disabled);
        }
    }

}