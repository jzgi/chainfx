using System.Text;

namespace Greatbone.Core
{
    ///
    /// The configurative settings for a web service.
    /// 
    /// <remark>
    /// The strong semantic allows the web folder hierarchy to be established during object initialization.
    /// </remark>
    /// <code>
    /// public class FooService : WebService
    /// {
    ///     public FooService(WebConfig cfg) : base(cfg)
    ///     {
    ///         Make&lt;BarFolder&gt;("bar");
    ///     }
    /// }
    /// </code>
    ///
    public class WebConfig : WebFolderContext, IData
    {
        /// The shard identifier when one service is divided into many shards
        public string shard;

        /// The bind addresses in URI (scheme://host:port) format.
        public string addresses;

        /// The services/addresses that this service references or depends on.
        public JObj cluster;

        /// The database connectivity attributes.
        public DbConfig db;

        /// The logging level, default to warning (3)
        public int logging = 3;

        /// Application-defined properties.
        public JObj extra;

        // connection string
        volatile string connstr;

        /// Let the file directory name same as the service name.
        public override string Directory => name;

        public JObj JObj { get; private set; }

        public bool? File { get; private set; }

        public WebConfig(string name)
        {
            this.name = name;
        }

        public void Load(ISource src, byte flags = 0)
        {
            src.Get(nameof(shard), ref shard);
            src.Get(nameof(addresses), ref addresses);
            src.Get(nameof(cluster), ref cluster);
            src.Get(nameof(db), ref db);
            src.Get(nameof(logging), ref logging);
            src.Get(nameof(extra), ref extra);
        }

        public void Dump<R>(ISink<R> snk, byte flags = 0) where R : ISink<R>
        {
            snk.Put(nameof(shard), shard);
            snk.Put(nameof(addresses), addresses);
            snk.Put(nameof(cluster), cluster);
            snk.Put(nameof(db), db);
            snk.Put(nameof(logging), logging);
            snk.Put(nameof(extra), extra);
        }

        ///
        /// Try to load from the $web.json file.
        ///
        public bool TryLoad()
        {
            string path = GetFilePath("$web.json");
            if (System.IO.File.Exists(path))
            {
                JObj jo = JsonUtility.FileToJObj(path);
                if (jo != null)
                {
                    JObj = jo;
                    Load(jo); // override
                    return (File = true).Value;
                }
            }
            return (File = false).Value;
        }

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
    }
}