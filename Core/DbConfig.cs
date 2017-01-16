namespace Greatbone.Core
{
    ///
    /// DB configuration embedded in WebConfig.
    ///
    public class DbConfig : IData
    {
        public string host;

        public int port;

        public string username;

        public string password;

        // whether to create event-queue tables/indexes
        public bool eq;

        public void Load(ISource src, byte flags = 0)
        {
            src.Get(nameof(host), ref host);
            src.Get(nameof(port), ref port);
            src.Get(nameof(username), ref username);
            src.Get(nameof(password), ref password);
            src.Get(nameof(eq), ref eq);
        }

        public void Dump<R>(ISink<R> snk, byte flags = 0) where R : ISink<R>
        {
            snk.Put(nameof(host), host);
            snk.Put(nameof(port), port);
            snk.Put(nameof(username), username);
            snk.Put(nameof(password), password);
            snk.Put(nameof(eq), eq);
        }
    }
}