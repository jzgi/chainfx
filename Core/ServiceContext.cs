using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// The configuration and running environment for a service istance. It is easy to be constructed programmatically or loaded from file.
    /// </summary>
    public class ServiceContext : WorkContext, IData
    {
        // the shard id of the service instance, can be null
        public string shard;

        // the bound addresses 
        public string[] addrs;

        // two ints for enc/dec authentication token
        public long cipher;

        // db configuration
        public Db db;

        // cluster members in the form of peerid-address pairs
        public Map<string, string> cluster;

        // logging level
        public int logging = 3;

        // shared cache or not
        public bool cache;

        public ServiceContext(string name) : base(name)
        {
        }

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            i.Get(nameof(shard), ref shard);
            i.Get(nameof(addrs), ref addrs);
            i.Get(nameof(db), ref db);
            i.Get(nameof(cluster), ref cluster);
            i.Get(nameof(logging), ref logging);
            i.Get(nameof(cache), ref cache);
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(shard), shard);
            o.Put(nameof(addrs), addrs);
            o.Put(nameof(db), db);
            o.Put(nameof(cluster), cluster);
            o.Put(nameof(logging), logging);
            o.Put(nameof(cache), cache);
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

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            i.Get(nameof(host), ref host);
            i.Get(nameof(port), ref port);
            i.Get(nameof(database), ref database);
            i.Get(nameof(username), ref username);
            i.Get(nameof(password), ref password);
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(host), host);
            o.Put(nameof(port), port);
            o.Put(nameof(database), database);
            o.Put(nameof(username), username);
            o.Put(nameof(password), password);
        }
    }
}