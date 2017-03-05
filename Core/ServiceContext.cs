namespace Greatbone.Core
{
    ///
    /// The configuration and running environment for a service istance. It is easy to be constructed programmatically or loaded from file.
    ///
    public class ServiceContext : FolderContext, IData
    {
        // the shard id of the service instance, can be null
        public string shard;

        // the bound addresses 
        public string[] addrs;

        // db configuration
        public Db db;

        // cluster members in the form of moniker-address pairs
        public Dict cluster;

        // logging level
        public int logging = 3;

        // authentication configuration
        public Auth auth;

        public ServiceContext(string name) : base(name)
        {
        }

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(shard), ref shard);
            i.Get(nameof(addrs), ref addrs);
            i.Get(nameof(db), ref db);
            i.Get(nameof(cluster), ref cluster);
            i.Get(nameof(logging), ref logging);
            i.Get(nameof(auth), ref auth);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(shard), shard);
            o.Put(nameof(addrs), addrs);
            o.Put(nameof(db), db);
            o.Put(nameof(cluster), cluster);
            o.Put(nameof(logging), logging);
            o.Put(nameof(auth), auth);
        }
    }

    ///
    /// The DB configuration embedded in a service context.
    ///
    public class Db : IData
    {
        // IP host or unix domain socket
        public string host;

        // IP port
        public int port;

        // default database name
        public string database;

        public string username;

        public string password;

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(host), ref host);
            i.Get(nameof(port), ref port);
            i.Get(nameof(database), ref database);
            i.Get(nameof(username), ref username);
            i.Get(nameof(password), ref password);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(host), host);
            o.Put(nameof(port), port);
            o.Put(nameof(database), database);
            o.Put(nameof(username), username);
            o.Put(nameof(password), password);
        }
    }

    ///
    /// The web authetication configuration embedded in a service context.
    ///
    public class Auth : IData
    {
        // mask for encoding/decoding token
        public int mask;

        // repositioning factor for encoding/decoding token
        public int pose;

        // The number of seconds that a signon durates, or null if session-wide.
        public int maxage;

        public string domain;

        // The service instance that does signon. Can be null if local
        public string moniker;

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(mask), ref mask);
            i.Get(nameof(pose), ref pose);
            i.Get(nameof(maxage), ref maxage);
            i.Get(nameof(domain), ref domain);
            i.Get(nameof(moniker), ref moniker);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(mask), mask);
            o.Put(nameof(pose), pose);
            o.Put(nameof(maxage), maxage);
            o.Put(nameof(domain), domain);
            o.Put(nameof(moniker), moniker);
        }
    }
}