using Greatbone.Core;
using static Greatbone.Core.XUtility;

namespace Greatbone.Sample
{

    /// <summary>
    /// Used by all services.
    /// </summary>
    public class Token : IPrincipal, IPersist
    {
        internal string id;
        internal string name;
        internal string credential;
        internal bool fame;
        internal bool brand;

        public string Key => id;

        public string Name => name;

        public string Credential => credential;

        public void Load(ISource s, byte x = 0xff)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(name), ref name);
            s.Get(nameof(credential), ref credential);
            s.Get(nameof(fame), ref fame);
            s.Get(nameof(brand), ref brand);
        }

        public void Dump<R>(ISink<R> s, byte x = 0xff) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            if (x.On(RESV))
                s.Put(nameof(credential), credential);
            s.Put(nameof(fame), fame);
            s.Put(nameof(brand), brand);
        }

    }

}