namespace Greatbone.Core
{
    public class DbConfig : ISerial
    {
        public string Host;

        public int Port;

        public string Username;

        public string Password;

        public bool MQ;

        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(Host), ref Host);
            r.Read(nameof(Port), ref Port);
            r.Read(nameof(Username), ref Username);
            r.Read(nameof(Password), ref Password);
            r.Read(nameof(MQ), ref MQ);
        }

        public void WriteTo(ISerialWriter w)
        {
            w.Write(nameof(Host), Host);
            w.Write(nameof(Port), Port);
            w.Write(nameof(Username), Username);
            w.Write(nameof(Password), Password);
            w.Write(nameof(MQ), MQ);
        }
    }

}