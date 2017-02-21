namespace Greatbone.Core
{
    ///
    /// The DB configuration embedded in a service context.
    ///
    public class DbConfig : IData
    {
        // IP host or unix domain socket
        public string host;

        // IP port
        public int port;

        // default database name
        public string database;

        public string username;

        public string password;

        // whether to create event-queue tables/indexes
        public bool queue;

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(host), ref host);
            i.Get(nameof(port), ref port);
            i.Get(nameof(database), ref database);
            i.Get(nameof(username), ref username);
            i.Get(nameof(password), ref password);
            i.Get(nameof(queue), ref queue);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(host), host);
            o.Put(nameof(port), port);
            o.Put(nameof(database), database);
            o.Put(nameof(username), username);
            o.Put(nameof(password), password);
            o.Put(nameof(queue), queue);
        }
    }
}