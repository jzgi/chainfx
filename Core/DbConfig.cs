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

        public void Load(ISource s, byte x = 0xff)
        {
            s.Get(nameof(host), ref host);
            s.Get(nameof(port), ref port);
            s.Get(nameof(username), ref username);
            s.Get(nameof(password), ref password);
            s.Get(nameof(msg), ref msg);
        }

        public void Dump<R>(ISink<R> s, byte x = 0xff) where R : ISink<R>
        {
            s.Put(nameof(host), host);
            s.Put(nameof(port), port);
            s.Put(nameof(username), username);
            s.Put(nameof(password), password);
            s.Put(nameof(msg), msg);
        }
        
    }

}