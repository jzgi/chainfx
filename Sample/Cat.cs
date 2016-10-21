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

        public void Load(ISource s, uint x = 0)
        {
            s.Got(nameof(id), ref id);
            s.Got(nameof(title), ref title);
            if (x.BinaryOn())
            {
                s.Got(nameof(img), ref img);
            }
            s.Got(nameof(filter), ref filter);
            s.Got(nameof(disabled), ref disabled);
        }

        public void Save<R>(ISink<R> s, uint x = 0) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(title), title);
            if (x.BinaryOn())
            {
                s.Put(nameof(img), img);}
            s.Put(nameof(filter), filter);
            s.Put(nameof(disabled), disabled);
        }
        
    }

}