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
    public class WebConfig : WebDirectoryContext, IData
    {
        /// The shard name when one service is divided into many shards
        public string shard;

        /// The outer socket address for external communication
        public string outer;

        /// The inner socket address for internal communication
        public string inner;

        /// All services (id-addres pairs) that this service referencesor depends on.
        public Obj refs;

        /// The database connectivity attributes.
        public DbConfig db;

        /// The logging level, default to warning (3)
        public int logging = 3;

        /// The URL of the signon interface.
        public string signon;

        public Obj extra;

        /// Let the folder name same ascthe service name.
        public override string Folder => name;


        public void Load(ISource s, byte z = 0)
        {
            s.Get(nameof(shard), ref shard);
            s.Get(nameof(outer), ref outer);
            s.Get(nameof(inner), ref inner);
            s.Get(nameof(refs), ref refs);
            s.Get(nameof(db), ref db);
            s.Get(nameof(logging), ref logging);
            s.Get(nameof(extra), ref extra);
        }

        public void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>
        {
            s.Put(nameof(shard), shard);
            s.Put(nameof(outer), outer);
            s.Put(nameof(inner), inner);
            s.Put(nameof(refs), refs);
            s.Put(nameof(logging), logging);
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