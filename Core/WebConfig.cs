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
    public class WebConfig : WebBuild, IPersist
    {
        // partition
        public string Part;

        // public socket address
        public string Public;

        // TLS or not
        public bool Tls;

        // private socket address
        public string Private;

        // private networking socket addresses
        public string[] Net;

        // database connectivity
        public DbConfig Db;

        // options
        public JObj Options;

        public void Load(ISource sc)
        {
            sc.Got(nameof(Key), ref Key);
            sc.Got(nameof(Part), ref Part);
            sc.Got(nameof(Public), ref Public);
            sc.Got(nameof(Tls), ref Tls);
            sc.Got(nameof(Private), ref Private);
            // sc.Get(nameof(Net), ref Net);
            sc.Got(nameof(Db), ref Db);
        }

        public void Save<R>(ISink<R> sk) where R : ISink<R>
        {
            sk.Put(nameof(Key), Key);
            sk.Put(nameof(Part), Part);
            sk.Put(nameof(Public), Public);
            sk.Put(nameof(Tls), Tls);
            sk.Put(nameof(Private), Private);
            // sk.Put(nameof(Net), Net);
            sk.Put(nameof(Db), Db);
        }

        public WebConfig LoadFile(string file)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonParse parse = new JsonParse(bytes);
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