using System.Text;

namespace Greatbone.Service
{
    /// <summary>
    /// The configuration for a service instance which is constructed programmatically or loaded from file.
    /// </summary>
    public class ServiceConfig : WorkConfig, IData
    {
        // the sharding notation for this service instance
        public string shard;

        // the descriptive information of this service instance
        public string descr;

        // the bound addresses 
        public string[] addrs;

        // two ints for enc/dec authentication token
        public long cipher;

        // certificate password if any
        public string certpass;

        // db configuration
        public Db db;

        // references to remote services in form of svcname[-idx] and url pairs
        public JObj refs;

        // logging level
        public int logging = 3;

        // shared cache or not
        public bool cache = true;

        public ServiceConfig(string name) : base(name)
        {
        }

        volatile string connstr;

        public string ConnectionString
        {
            get
            {
                if (connstr == null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Host=").Append(db.host);
                    sb.Append(";Port=").Append(db.port);
                    sb.Append(";Database=").Append(db.database ?? Name);
                    sb.Append(";Username=").Append(db.username);
                    sb.Append(";Password=").Append(db.password);
                    sb.Append(";Read Buffer Size=").Append(1024 * 32);
                    sb.Append(";Write Buffer Size=").Append(1024 * 32);
                    sb.Append(";No Reset On Close=").Append(true);

                    connstr = sb.ToString();
                }
                return connstr;
            }
        }

        public virtual void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(shard), ref shard);
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(addrs), ref addrs);
            s.Get(nameof(cipher), ref cipher);
            s.Get(nameof(certpass), ref certpass);
            s.Get(nameof(db), ref db);
            s.Get(nameof(refs), ref refs);
            s.Get(nameof(logging), ref logging);
            s.Get(nameof(cache), ref cache);
        }

        public virtual void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(shard), shard);
            s.Put(nameof(descr), descr);
            s.Put(nameof(addrs), addrs);
            s.Put(nameof(cipher), cipher);
            s.Put(nameof(certpass), certpass);
            s.Put(nameof(db), db);
            s.Put(nameof(refs), refs);
            s.Put(nameof(logging), logging);
            s.Put(nameof(cache), cache);
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

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(host), ref host);
            s.Get(nameof(port), ref port);
            s.Get(nameof(database), ref database);
            s.Get(nameof(username), ref username);
            s.Get(nameof(password), ref password);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(host), host);
            s.Put(nameof(port), port);
            s.Put(nameof(database), database);
            s.Put(nameof(username), username);
            s.Put(nameof(password), password);
        }
    }
}