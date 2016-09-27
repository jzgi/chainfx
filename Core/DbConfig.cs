namespace Greatbone.Core
{
    public class DbConfig : IDat
    {
        public string Host;

        public int Port;

        public string Username;

        public string Password;

        public bool MQ;

        public void From(IInput r)
        {
            r.Get(nameof(Host), ref Host);
            r.Get(nameof(Port), ref Port);
            r.Get(nameof(Username), ref Username);
            r.Get(nameof(Password), ref Password);
            r.Get(nameof(MQ), ref MQ);
        }

        public void To(IOutput w)
        {
            w.Put(nameof(Host), Host);
            w.Put(nameof(Port), Port);
            w.Put(nameof(Username), Username);
            w.Put(nameof(Password), Password);
            w.Put(nameof(MQ), MQ);
        }
    }

}