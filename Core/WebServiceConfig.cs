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
    public class WebServiceConfig : WebConfig, ISerial
    {
        ///<summary>Z-axis scaling</summary>
        public string Shard;

        // public socket address
        public string Public;

        public bool Tls;

        // private socket address for clustering
        public string Private;

        // event system socket addresses
        public List<string> Cluster;

        public DbConfig Db;

        public Dictionary<string, string> options;

        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(Key), ref Key);
            r.Read(nameof(Shard), ref Shard);
            r.Read(nameof(Public), ref Public);
            r.Read(nameof(Tls), ref Tls);
            r.Read(nameof(Private), ref Private);
            r.Read(nameof(Cluster), ref Cluster);
            r.Read(nameof(Db), ref Db);
            r.Read(nameof(Debug), ref Debug);
            r.Read(nameof(options), ref options);
        }

        public void WriteTo(ISerialWriter w)
        {
            w.Write(nameof(Key), Key);
            w.Write(nameof(Shard), Shard);
            w.Write(nameof(Public), Public);
            w.Write(nameof(Tls), Tls);
            w.Write(nameof(Private), Private);
            w.Write(nameof(Cluster), Cluster);
            w.Write(nameof(Db), Db);
            w.Write(nameof(Debug), Debug);
            w.Write(nameof(options), options);
        }

        public WebServiceConfig Load(string file)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonContent jc = new JsonContent(bytes, bytes.Length);

                jc.ReadObject(() =>
                {
                    ReadFrom(jc);
                });
            }
            catch
            {
            }
            return this;
        }
    }

}