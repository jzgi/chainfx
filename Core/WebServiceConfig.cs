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
    public class WebServiceConfig : WebSubConfig, ISerial
    {
        internal string SubKey;

        // public socket address
        internal string Public;

        internal bool Tls;

        // private socket address for clustering
        internal string Private;

        // event system socket addresses
        internal string[] Cluster;

        internal DbConfig Db;

        internal Dictionary<string, string> options;

        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(Key), ref Key);
            r.Read(nameof(SubKey), ref SubKey);
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
            w.Write(nameof(SubKey), SubKey);
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