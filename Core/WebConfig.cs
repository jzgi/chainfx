namespace Greatbone.Core
{

    ///
    /// <summary>
    /// The configurative settings for a web service.
    /// </summary>
    /// <remarks>
    /// It provides a strong semantic that enables the whole controller hierarchy to be established during object initialization.
    /// </remarks>
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
    public class WebConfig : WebNodeContext, IBean
    {

        // partition
        internal string part;

        // socket address for external access
        internal string @extern;

        // TLS or not
        internal bool tls;

        // socket address for internal access
        internal string intern;

        // intranet socket addresses
        internal string[] net;

        // database connectivity
        internal DbConfig db;

        // logging level, default to warning (3)
        internal int logging = 3;

        internal Obj opts;

        public Obj Opts => opts;

        // ovveride to provide a decent folder service name
        public override string Folder => key;


        public void Load(ISource s, byte z = 0)
        {
            s.Get(nameof(part), ref part);
            s.Get(nameof(@extern), ref @extern);
            s.Get(nameof(tls), ref tls);
            s.Get(nameof(intern), ref intern);
            s.Get(nameof(net), ref net);
            s.Get(nameof(db), ref db);
            s.Get(nameof(logging), ref logging);
            s.Get(nameof(opts), ref opts);
        }

        public void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>
        {
            s.Put(nameof(part), part);
            s.Put(nameof(@extern), @extern);
            s.Put(nameof(tls), tls);
            s.Put(nameof(intern), intern);
            s.Put(nameof(net), net);
            s.Put(nameof(logging), logging);
            s.Put(nameof(opts), opts);
        }

        public WebConfig Load()
        {
            if (key == null) throw new WebException("missing key");

            Obj jo = JsonUtility.FileToObj(GetFilePath("$web.json"));
            if (jo != null)
            {
                Load(jo); // override
            }
            return this;
        }

    }

}