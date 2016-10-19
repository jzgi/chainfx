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

        public void Load(ISource s, uint x = 0)
        {
            s.Got(nameof(id), ref id);
            s.Got(nameof(title), ref title);
            s.Got(nameof(filter), ref filter);
            s.Got(nameof(disabled), ref disabled);
        }

        public void Save<R>(ISink<R> s, uint x = 0) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(title), title);
            s.Put(nameof(filter), filter);
            s.Put(nameof(disabled), disabled);
        }
    }

}