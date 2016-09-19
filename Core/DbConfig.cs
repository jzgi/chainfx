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
            r.Read(nameof(Host), out Host);
            r.Read(nameof(Port), out Port);
            r.Read(nameof(Username), out Username);
            r.Read(nameof(Password), out Password);
            r.Read(nameof(MQ), out MQ);
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