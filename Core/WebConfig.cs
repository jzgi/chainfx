namespace Greatbone.Core
{
    ///
    /// The configurative settings for a web service.
    /// 
    /// <remark>
    /// It provides a strong semantic that enables the whole controller hierarchy to be established during object initialization.
    /// </remark>
    /// <code>
    /// public class FooService : WebService
    /// {
    ///         public FooService(WebConfig cfg) : base(cfg)
    ///         {
    ///                 AddChild&lt;BarControl&gt;();
    ///         }
    /// }
    /// </code>
    ///
    public class WebConfig : WebMakeContext, IData
    {
        // can be null
        public string partition;

        // outer  socket address
        public string outer;

        // inner socket address
        public string inner;

        // peer services this service references
        public Obj refs;

        // database connectivity
        public DbConfig db;

        // logging level, default to warning (3)
        public int logging = 3;

        public Obj extra;

        // ovveride to provide a decent folder service name
        public override string Folder => name;


        public void Load(ISource s, byte z = 0)
        {
            s.Get(nameof(partition), ref partition);
            s.Get(nameof(outer), ref outer);
            s.Get(nameof(inner), ref inner);
            s.Get(nameof(refs), ref refs);
            s.Get(nameof(db), ref db);
            s.Get(nameof(logging), ref logging);
            s.Get(nameof(extra), ref extra);
        }

        public void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>
        {
            s.Put(nameof(partition), partition);
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