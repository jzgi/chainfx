namespace Greatbone.Core
{
    public class DbConfig : ISerial
    {
        internal string host;

        internal int port;

        internal string username;

        internal string password;

        internal bool msg;

        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(host), ref host);
            r.Read(nameof(port), ref port);
            r.Read(nameof(username), ref username);
            r.Read(nameof(password), ref password);
            r.Read(nameof(msg), ref msg);
        }

        public void WriteTo(ISerialWriter w)
        {
            w.Write(nameof(host), host);
            w.Write(nameof(port), port);
            w.Write(nameof(username), username);
            w.Write(nameof(password), password);
            w.Write(nameof(msg), msg);
        }
    }

}