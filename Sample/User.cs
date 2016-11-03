using System;
using Greatbone.Core;
using static Greatbone.Core.XUtility;

namespace Greatbone.Sample
{

    ///
    public class User : IPersist
    {
        internal string id;
        internal string name;
        internal string credential;
        internal bool fame;
        internal bool brand;
        internal DateTime date;

        public void Load(ISource s, byte x = 0)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(name), ref name);
            if (x.Ya(RESV)) s.Get(nameof(credential), ref credential);
            s.Get(nameof(fame), ref fame);
            s.Get(nameof(brand), ref brand);
            s.Get(nameof(date), ref date);
        }

        public void Dump<R>(ISink<R> s, byte x = 0) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            if (x.Ya(RESV)) s.Put(nameof(credential), credential);
            s.Put(nameof(fame), fame);
            s.Put(nameof(brand), brand);
            s.Put(nameof(date), date);
        }

    }

}