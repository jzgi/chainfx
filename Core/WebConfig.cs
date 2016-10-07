using System;
using System.Diagnostics;
using System.IO;

namespace Greatbone.Core
{
    /// <summary>The configurative settings and the establishment of creation context during initialization of the controller hierarchy.</summary>
    /// <remarks>It provides a strong semantic that enables the whole controller hierarchy to be established within execution of constructors, starting from the constructor of a service controller.</remarks>
    /// <example>
    /// public class FooService : WebService
    /// {
    ///         public FooService(WebConfig cfg) : base(wsc)
    ///         {
    ///                 AddSub&lt;BarSub&gt;();
    ///         }
    /// }
    /// </example>
    ///
    public class WebConfig : WebTie, IPersist
    {
        // partition
        internal string part;

        // public socket address
        internal string @public;

        // TLS or not
        internal bool tls;

        // private socket address
        internal string @private;

        // private networking socket addresses
        internal string[] net;

        // database connectivity
        internal DbConfig db;

        // options
        internal JObj options;

        public void Load(ISource sc)
        {
            sc.Got(nameof(key), ref key);
            sc.Got(nameof(part), ref part);
            sc.Got(nameof(@public), ref @public);
            sc.Got(nameof(tls), ref tls);
            sc.Got(nameof(@private), ref @private);
            // sc.Get(nameof(Net), ref Net);
            sc.Got(nameof(db), ref db);
        }

        public void Save<R>(ISink<R> sk) where R : ISink<R>
        {
            sk.Put(nameof(key), key);
            sk.Put(nameof(part), part);
            sk.Put(nameof(@public), @public);
            sk.Put(nameof(tls), tls);
            sk.Put(nameof(@private), @private);
            // sk.Put(nameof(Net), Net);
            sk.Put(nameof(db), db);
        }

        public WebConfig LoadFile(string file)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JParse parse = new JParse(bytes);
                JObj obj = (JObj)parse.Parse();

                Load(obj); // may override

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
            return this;
        }
    }

}