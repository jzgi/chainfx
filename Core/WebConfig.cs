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
    public class WebConfig : WebMake, IData
    {
        // service subkey, can be null
        public string subkey;

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
        public override string Folder => key;


        public void Load(ISource s, byte z = 0)
        {
            s.Get(nameof(subkey), ref subkey);
            s.Get(nameof(outer), ref outer);
            s.Get(nameof(inner), ref inner);
            s.Get(nameof(refs), ref refs);
            s.Get(nameof(db), ref db);
            s.Get(nameof(logging), ref logging);
            s.Get(nameof(extra), ref extra);
        }

        public void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>
        {
            s.Put(nameof(subkey), subkey);
            s.Put(nameof(outer), outer);
            s.Put(nameof(inner), inner);
            s.Put(nameof(refs), refs);
            s.Put(nameof(logging), logging);
            s.Put(nameof(extra), extra);
        }

        public WebConfig Load()
        {
            if (key == null) throw new WebException("missing key");

            Obj obj = JsonUtility.FileToObj(GetFilePath("$web.json"));
            if (obj != null)
            {
                Load(obj); // override
            }
            return this;
        }
    }
}