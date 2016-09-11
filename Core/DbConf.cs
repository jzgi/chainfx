namespace Greatbone.Core
{
    public class DbConf : ISerial
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