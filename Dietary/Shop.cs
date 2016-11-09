using Greatbone.Core;

namespace Ministry.Dietary
{

    /// <summary>
    /// A login internal user.
    /// </summary>
    public class Shop : IPrincipal, IBean
    {
        internal string id;
        internal string name;
        internal string credential;
        internal string tel;
        internal string addr;

        public string Key => id;

        public string Name => name;

        public string Credential => credential;

        public void Load(ISource s, byte z = 0)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(name), ref name);
            s.Get(nameof(credential), ref credential);
            s.Get(nameof(tel), ref tel);
            s.Get(nameof(addr), ref addr);
        }

        public void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            s.Put(nameof(credential), credential);
            s.Put(nameof(tel), tel);
            s.Put(nameof(addr), addr);
        }

    }

}