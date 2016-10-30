namespace Greatbone.Core
{
    /// <summary>
    /// The configurative settings and the establishment of creation context during initialization of the controller hierarchy.
    /// </summary>
    /// <remarks>
    /// It provides a strong semantic that enables the whole controller hierarchy to be established within execution of constructors, starting from the constructor of a service controller.
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
    public class WebConfig : WebArg, IPersist
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

        internal JObj opts;

        public JObj Opts => opts;

        // ovveride to provide a decent folder service name
        public override string Folder => key;


        public void Load(ISource s, byte x = 0xff)
        {
            s.Got(nameof(part), ref part);
            s.Got(nameof(@extern), ref @extern);
            s.Got(nameof(tls), ref tls);
            s.Got(nameof(intern), ref intern);
            s.Got(nameof(net), ref net);
            s.Got(nameof(db), ref db);
            s.Got(nameof(logging), ref logging);
            s.Got(nameof(opts), ref opts);
        }

        public void Save<R>(ISink<R> s, byte x = 0xff) where R : ISink<R>
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

            JObj jo = JUtility.FileToJObj(GetFilePath("$web.json"));
            if (jo != null)
            {
                Load(jo); // override
            }
            return this;
        }

    }

}