namespace Greatbone.Core
{
    public class DbConfig : IPersist
    {
        internal string host;

        internal int port;

        internal string username;

        internal string password;

        // whether to create message tables
        internal bool msg;

        public void Load(ISource sc, ushort x = 0)
        {
            sc.Got(nameof(host), ref host);
            sc.Got(nameof(port), ref port);
            sc.Got(nameof(username), ref username);
            sc.Got(nameof(password), ref password);
            sc.Got(nameof(msg), ref msg);
        }

        public void Save<R>(ISink<R> k, ushort x = 0) where R : ISink<R>
        {
            k.Put(nameof(host), host);
            k.Put(nameof(port), port);
            k.Put(nameof(username), username);
            k.Put(nameof(password), password);
            k.Put(nameof(msg), msg);
        }
    }

}