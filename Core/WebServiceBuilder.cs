using System.IO;

namespace Greatbone.Core
{
    public class WebServiceBuilder : WebServiceContext, ISerial
    {
        internal DataSrcBuilder datasrc;


        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(key), ref key);
            r.Read(nameof(@public), ref @public);
            r.Read(nameof(tls), ref tls);
            r.Read(nameof(@internal), ref @internal);
            r.Read(nameof(foreign), ref foreign);
            r.Read(nameof(datasrc), ref datasrc);
            r.Read(nameof(debug), ref debug);
            r.Read(nameof(options), ref options);
        }

        public void WriteTo(ISerialWriter w)
        {
            w.Write(nameof(key), key);
            w.Write(nameof(@public), @public);
            w.Write(nameof(tls), tls);
            w.Write(nameof(@internal), @internal);
            w.Write(nameof(foreign), foreign);
            w.Write(nameof(datasrc), datasrc);
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
                    JsonText text = new JsonText(json);

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

    public class DataSrcBuilder : ISerial
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