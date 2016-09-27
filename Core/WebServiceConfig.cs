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
    public class WebServiceConfig : WebConfig, IDat
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

        public void From(IInput r)
        {
            r.Get(nameof(Key), ref Key);
            r.Get(nameof(Part), ref Part);
            r.Get(nameof(Public), ref Public);
            r.Get(nameof(Tls), ref Tls);
            r.Get(nameof(Private), ref Private);
            r.Get(nameof(Net), ref Net);
            r.Get(nameof(Db), ref Db);
            r.Get(nameof(Debug), ref Debug);
            r.Get(nameof(options), ref options);
        }

        public void To(IOutput w)
        {
            w.Put(nameof(Key), Key);
            w.Put(nameof(Part), Part);
            w.Put(nameof(Public), Public);
            w.Put(nameof(Tls), Tls);
            w.Put(nameof(Private), Private);
            w.Put(nameof(Net), Net);
            w.Put(nameof(Db), Db);
            w.Put(nameof(Debug), Debug);
            w.Put(nameof(options), options);
        }

        public WebServiceConfig Load(string file)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonContent jc = new JsonContent(bytes, bytes.Length);

            }
            catch
            {
            }
            return this;
        }
    }

}