using System;
using System.Collections.Generic;
using System.IO;

namespace Greatbone.Core
{
    /// <summary>The configurative settings and the establishment of creation context during initialization of the controller hierarchy.</summary>
    /// <remarks>It provides a strong semantic that enables the whole controller hierarchy to be established within execution of constructors, starting from the constructor of a service controller.</remarks>
    /// <example>
    /// public class FooService : WebService
    /// {
    ///         public FooService(WebServiceBuilder wsb) : base(wsc)
    ///         {
    ///                 AddSub&lt;BarSub&gt;();
    ///         }
    /// }
    /// </example>
    ///
    public class WebServiceConfig : WebConfig, IPersist
    {
        ///<summary>Z-axis scaling</summary>
        public string Part;

        // public socket address
        public string Public;

        public bool Tls;

        // private socket address for clustering
        public string Private;

        // event system socket addresses
        public List<string> Net;

        public DbConfig Db;

        public Dictionary<string, string> options;

        public void Load(ISource sc, int x)
        {
            sc.Get(nameof(Key), ref Key);
            sc.Get(nameof(Part), ref Part);
            sc.Get(nameof(Public), ref Public);
            sc.Get(nameof(Tls), ref Tls);
            sc.Get(nameof(Private), ref Private);
            sc.Get(nameof(Net), ref Net);
            sc.Get(nameof(Db), ref Db);
            sc.Get(nameof(Debug), ref Debug);
            sc.Get(nameof(options), ref options);
        }

        public void Save<R>(ISink<R> sk, int x) where R : ISink<R>
        {
            sk.Put(nameof(Key), Key);
            sk.Put(nameof(Part), Part);
            sk.Put(nameof(Public), Public);
            sk.Put(nameof(Tls), Tls);
            sk.Put(nameof(Private), Private);
            sk.Put(nameof(Net), Net);
            sk.Put(nameof(Db), Db);
            sk.Put(nameof(Debug), Debug);
            sk.Put(nameof(options), options);
        }

        public WebServiceConfig Load(string file)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonParse parser = new JsonParse(bytes);
                Obj obj = (Obj)parser.Parse();

                Load(obj, 0); // may override

            }
            catch
            {
            }
            return this;
        }
    }

}