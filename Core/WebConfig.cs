using System.IO;

namespace Greatbone.Core
{
    ///
    /// The configurative settings for a web service.
    /// 
    /// <remark>
    /// The strong semantic allows the entire directory hierarchy to be established during object initialization.
    /// </remark>
    /// <code>
    /// public class FooService : WebService
    /// {
    ///     public FooService(WebConfig cfg) : base(cfg)
    ///     {
    ///         Make&lt;BarDirectory&gt;("bar");
    ///     }
    /// }
    /// </code>
    ///
    public class WebConfig : WebDirectoryContext, IDat
    {
        /// The shard name when one service is divided into many shards
        public string shard;

        /// The outer socket address for external communication
        public string outer;

        /// The inner socket address for internal communication
        public string inner;

        /// The cookie domain to apply
        public string domain;

        /// The absolute or relative URL of the signon user interface.
        public string signon;

        /// The services/addresses that this service references or depends on.
        public Obj refs;

        /// The database connectivity attributes.
        public DbConfig db;

        /// The logging level, default to warning (3)
        public int logging = 3;

        /// Turn on/off the response cache.
        public bool cache = true;

        /// Let the folder name same ascthe service name.
        public override string Folder => name;

        public Obj WebJson { get; private set; }

        public WebConfig(string name)
        {
            this.name = name;
        }

        public void Load(ISource src, byte z = 0)
        {
            src.Get(nameof(shard), ref shard);
            src.Get(nameof(outer), ref outer);
            src.Get(nameof(inner), ref inner);
            src.Get(nameof(domain), ref domain);
            src.Get(nameof(signon), ref signon);
            src.Get(nameof(refs), ref refs);
            src.Get(nameof(db), ref db);
            src.Get(nameof(logging), ref logging);
            src.Get(nameof(cache), ref cache);
        }

        public void Dump<R>(ISink<R> snk, byte z = 0) where R : ISink<R>
        {
            snk.Put(nameof(shard), shard);
            snk.Put(nameof(outer), outer);
            snk.Put(nameof(inner), inner);
            snk.Put(nameof(domain), domain);
            snk.Put(nameof(signon), signon);
            snk.Put(nameof(refs), refs);
            snk.Put(nameof(db), db);
            snk.Put(nameof(logging), logging);
            snk.Put(nameof(cache), cache);
        }

        public bool LoadWebJson()
        {
            string path = GetFilePath("$web.json");
            if (System.IO.File.Exists(path))
            {
                Obj obj = JsonUtility.FileToObj(path);
                if (obj != null)
                {
                    WebJson = obj;
                    Load(obj); // override
                    return true;
                }
            }
            return false;
        }
    }
}