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
    public class WebServiceBuilder : WebServiceContext, ISerial
    {
        // public socket address
        internal string outer;

        internal bool tls;

        // inner socket address
        internal string inner;

        // event system socket addresses
        internal string[] foreign;

        internal DbBuilder db;

        internal Dictionary<string, string> options;

        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(key), ref key);
            r.Read(nameof(outer), ref outer);
            r.Read(nameof(tls), ref tls);
            r.Read(nameof(inner), ref inner);
            r.Read(nameof(foreign), ref foreign);
            r.Read(nameof(db), ref db);
            r.Read(nameof(debug), ref debug);
            r.Read(nameof(options), ref options);
        }

        public void WriteTo(ISerialWriter w)
        {
            w.Write(nameof(key), key);
            w.Write(nameof(outer), outer);
            w.Write(nameof(tls), tls);
            w.Write(nameof(inner), inner);
            w.Write(nameof(foreign), foreign);
            w.Write(nameof(db), db);
            w.Write(nameof(debug), debug);
            w.Write(nameof(options), options);
        }

        public WebServiceBuilder Load(string file)
        {
            try
            {
                string json = File.ReadAllText(file);
                if (json != null || json == null)
                {
                    JsonTextReader text = new JsonTextReader(json);

                    text.ReadLeft();
//                ReadFrom(text);
                    text.ReadRight();

                    if (key == null)
                    {
                        key = Path.GetFileNameWithoutExtension(file);
                    }
                }
            }
            catch
            {
            }
            return this;
        }
    }

    public class DbBuilder : ISerial
    {
        internal string host;

        internal int port;

        internal string username;

        internal string password;

        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(host), ref host);
            r.Read(nameof(port), ref port);
            r.Read(nameof(username), ref username);
            r.Read(nameof(password), ref password);
        }

        public void WriteTo(ISerialWriter w)
        {
            w.Write(nameof(host), host);
            w.Write(nameof(port), port);
            w.Write(nameof(username), username);
            w.Write(nameof(password), password);
        }
    }
}