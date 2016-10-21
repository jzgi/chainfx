using System;
using Greatbone.Core;

namespace Greatbone.Sample
{

    ///
    public class User : IPersist
    {
        public const uint NoDefault = 2, Extra = 4;

        internal string id;
        internal string name;
        internal string credential;
        internal bool fame;
        internal bool brand;
        internal bool admin;
        internal DateTime date;

        public void Load(ISource s, uint x = 0)
        {
            s.Got(nameof(id), ref id);
            s.Got(nameof(name), ref name);
            s.Got(nameof(credential), ref credential);
            s.Got(nameof(fame), ref fame);
            s.Got(nameof(brand), ref brand);
            s.Got(nameof(admin), ref admin);
            s.Got(nameof(date), ref date);
        }

        public void Save<R>(ISink<R> s, uint x = 0) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            s.Put(nameof(credential), credential);
            s.Put(nameof(fame), fame);
            s.Put(nameof(brand), brand);
            s.Put(nameof(admin), admin);
            s.Put(nameof(date), date);
        }

    }
}