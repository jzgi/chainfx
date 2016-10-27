using Greatbone.Core;
using static Greatbone.Core.XUtility;

namespace Greatbone.Sample
{

    /// <summary>
    /// Used by all services.
    /// </summary>
    public class Token : IToken, IPersist
    {
        internal string id;
        internal string name;
        internal string credential;
        internal bool fame;
        internal bool brand;
        internal bool admin;

        public string Key => id;

        public string Name => name;

        public void Load(ISource s, byte x = 0xff)
        {
            s.Got(nameof(id), ref id);
            s.Got(nameof(name), ref name);
            s.Got(nameof(credential), ref credential);
            s.Got(nameof(fame), ref fame);
            s.Got(nameof(brand), ref brand);
            s.Got(nameof(admin), ref admin);
        }

        public void Save<R>(ISink<R> s, byte x = 0xff) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            if (x.On(RESV))
                s.Put(nameof(credential), credential);
            s.Put(nameof(fame), fame);
            s.Put(nameof(brand), brand);
            s.Put(nameof(admin), admin);
        }

    }
}