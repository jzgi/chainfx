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
        public string[] Cluster;

        public DbConfig Db;

        public Dictionary<string, string> options;

        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(Key), out Key);
            r.Read(nameof(Shard), out Shard);
            r.Read(nameof(Public), out Public);
            r.Read(nameof(Tls), out Tls);
            r.Read(nameof(Private), out Private);
            r.Read(nameof(Cluster), out Cluster);
            r.Read(nameof(Db), out Db);
            r.Read(nameof(Debug), out Debug);
            r.Read(nameof(options), out options);
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
                string json = File.ReadAllText(file);
                if (json != null || json == null)
                {
                    JsonText text = new JsonText(json);

                    text.ReadObject(() =>
                    {

                    });
                    if (Key == null)
                    {
                        Key = Path.GetFileNameWithoutExtension(file);
                    }
                }
            }
            catch
            {
            }
            return this;
        }
    }

}