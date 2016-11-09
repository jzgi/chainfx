using Greatbone.Core;

namespace Ministry.Dietary
{

    /// <summary>
    /// An administrator's login.
    /// </summary>
    public class Login : IPrincipal, IBean
    {
        internal string id;
        internal string name;
        internal string credential;
        internal string[] roles;

        public string Key => id;

        public string Name => name;

        public string Credential => credential;

        public void Load(ISource s, byte z = 0)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(name), ref name);
            s.Get(nameof(credential), ref credential);
            s.Get(nameof(roles), ref roles);
        }

        public void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            s.Put(nameof(credential), credential);
            s.Put(nameof(roles), roles);
        }

    }

}