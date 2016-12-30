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

        /// The open socket address for external communication
        public string pub;

        /// The internal socket address for internal communication
        public string intern;

        /// The services/addresses that this service references or depends on.
        public Obj cluster;

        /// The database connectivity attributes.
        public DbConfig db;

        /// The logging level, default to warning (3)
        public int logging = 3;

        /// Turn on/off the response cache.
        public bool cache = true;

        /// Let the file directory name same as the service name.
        public override string Directory => name;

        public Obj Json { get; private set; }

        public WebConfig(string name)
        {
            this.name = name;
        }

        public void Load(ISource src, byte bits = 0)
        {
            src.Get(nameof(shard), ref shard);
            src.Get(nameof(pub), ref pub);
            src.Get(nameof(intern), ref intern);
            src.Get(nameof(cluster), ref cluster);
            src.Get(nameof(db), ref db);
            src.Get(nameof(logging), ref logging);
            src.Get(nameof(cache), ref cache);
        }

        public void Dump<R>(ISink<R> snk, byte bits = 0) where R : ISink<R>
        {
            snk.Put(nameof(shard), shard);
            snk.Put(nameof(pub), pub);
            snk.Put(nameof(intern), intern);
            snk.Put(nameof(cluster), cluster);
            snk.Put(nameof(db), db);
            snk.Put(nameof(logging), logging);
            snk.Put(nameof(cache), cache);
        }

        public bool LoadJson()
        {
            string path = GetFilePath("$web.json");
            if (System.IO.File.Exists(path))
            {
                Obj obj = JsonUtility.FileToObj(path);
                if (obj != null)
                {
                    Json = obj;
                    Load(obj); // override
                    return true;
                }
            }
            return false;
        }
    }
}