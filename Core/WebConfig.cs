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

        public Obj extra;

        /// Let the folder name same ascthe service name.
        public override string Folder => name;


        public void Load(ISource s, byte z = 0)
        {
            s.Get(nameof(shard), ref shard);
            s.Get(nameof(outer), ref outer);
            s.Get(nameof(inner), ref inner);
            s.Get(nameof(domain), ref domain);
            s.Get(nameof(signon), ref signon);
            s.Get(nameof(refs), ref refs);
            s.Get(nameof(db), ref db);
            s.Get(nameof(logging), ref logging);
            s.Get(nameof(cache), ref cache);
            s.Get(nameof(extra), ref extra);
        }

        public void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>
        {
            s.Put(nameof(shard), shard);
            s.Put(nameof(outer), outer);
            s.Put(nameof(inner), inner);
            s.Put(nameof(domain), domain);
            s.Put(nameof(signon), signon);
            s.Put(nameof(refs), refs);
            s.Put(nameof(db), db);
            s.Put(nameof(logging), logging);
            s.Put(nameof(cache), cache);
            s.Put(nameof(extra), extra);
        }

        public WebConfig Load()
        {
            if (name == null) throw new WebException("missing key");

            Obj obj = JsonUtility.FileToObj(GetFilePath("$web.json"));
            if (obj != null)
            {
                Load(obj); // override
            }
            return this;
        }
    }
}