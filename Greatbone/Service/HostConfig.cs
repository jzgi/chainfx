using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Greatbone.Service
{
    /// <summary>
    /// The configuration for an applicaion.
    /// </summary>
    public class HostConfig : IData
    {
        // the sharding notation for this service instance
        public string shard;

        // the descriptive information of this service instance
        public string descr;

        // logging level
        public int logging = 3;

        public int cipher;

        public string certpasswd;

        // the bound addresses 
        public Web web;

        // db configuration
        public Db db;

        // references to remote services in form of svcname[-idx] and url pairs
        public JObj @ref;


        X509Certificate2 certificate;

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
                    sb.Append(";Database=").Append(db.database);
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

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(shard), ref shard);
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(logging), ref logging);
            s.Get(nameof(cipher), ref cipher);
            s.Get(nameof(certpasswd), ref certpasswd);
            s.Get(nameof(web), ref web);
            s.Get(nameof(db), ref db);
            s.Get(nameof(@ref), ref @ref);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(shard), shard);
            s.Put(nameof(descr), descr);
            s.Put(nameof(logging), logging);
            s.Put(nameof(cipher), cipher);
            s.Put(nameof(certpasswd), certpasswd);
            s.Put(nameof(web), web);
            s.Put(nameof(db), db);
            s.Put(nameof(@ref), @ref);
        }

        ///
        /// The DB configuration
        ///
        public class Web : IData
        {
            // the bound addresses 
            public string[] addrs;

            // two ints for enc/dec authentication token
            public long cipher;

            // certificate password if any
            public string certpasswd;

            // shared cache or not
            public bool cache = true;

            public void Read(ISource s, byte proj = 0x0f)
            {
                s.Get(nameof(addrs), ref addrs);
                s.Get(nameof(cipher), ref cipher);
                s.Get(nameof(certpasswd), ref certpasswd);
                s.Get(nameof(cache), ref cache);
            }

            public void Write(ISink s, byte proj = 0x0f)
            {
                s.Put(nameof(addrs), addrs);
                s.Put(nameof(cipher), cipher);
                s.Put(nameof(certpasswd), certpasswd);
                s.Put(nameof(cache), cache);
            }
        }

        ///
        /// The DB configuration
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
}