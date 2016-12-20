using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// An administrator's login.
    ///
    public class ShopToken : IPrincipal, IData
    {
        internal string id;
        internal string name;
        internal string credential;
        internal short level;

        public string Key => id;

        public string Name => name;

        public string Credential => credential;

        public void Load(ISource s, byte z = 0)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(name), ref name);
            s.Get(nameof(credential), ref credential);
            s.Get(nameof(level), ref level);
        }

        public void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            s.Put(nameof(credential), credential);
            s.Put(nameof(level), level);
        }
    }
}