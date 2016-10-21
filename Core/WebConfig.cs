using System;
using System.Diagnostics;
using System.IO;

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
    ///                 AddControl&lt;BarSub&gt;();
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

        // logging level
        internal int logging;

        internal JObj opts;

        public JObj Opts => opts;

        // ovveride to provide a decent folder service name
        public override string Folder => key;


        public void Load(ISource s, uint x = 0)
        {
            s.Got(nameof(key), ref key);
            s.Got(nameof(part), ref part);
            s.Got(nameof(@extern), ref @extern);
            s.Got(nameof(tls), ref tls);
            s.Got(nameof(intern), ref intern);
            s.Got(nameof(net), ref net);
            s.Got(nameof(db), ref db);
            s.Got(nameof(logging), ref logging);
            s.Got(nameof(opts), ref opts);
        }

        public void Save<R>(ISink<R> s, uint x = 0) where R : ISink<R>
        {
            s.Put(nameof(key), key);
            s.Put(nameof(part), part);
            s.Put(nameof(@extern), @extern);
            s.Put(nameof(tls), tls);
            s.Put(nameof(intern), intern);
            s.Put(nameof(net), net);
            s.Put(nameof(logging), logging);
            s.Put(nameof(opts), opts);
        }

        public WebConfig LoadFile(string file)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JParse parse = new JParse(bytes, bytes.Length);
                JObj jo = (JObj)parse.Parse();

                Load(jo); // may override

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
            return this;
        }
    }

}