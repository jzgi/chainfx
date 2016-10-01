namespace Greatbone.Core
{
    public class DbConfig : IPersist
    {
        public string Host;

        public int Port;

        public string Username;

        public string Password;

        public bool MQ;

        public void Load(ISource c, int fs)
        {
            c.Get(nameof(Host), ref Host);
            c.Get(nameof(Port), ref Port);
            c.Get(nameof(Username), ref Username);
            c.Get(nameof(Password), ref Password);
            c.Get(nameof(MQ), ref MQ);
        }

        public void Save<R>(ISink<R> k, int fs) where R : ISink<R>
        {
            k.Put(nameof(Host), Host);
            k.Put(nameof(Port), Port);
            k.Put(nameof(Username), Username);
            k.Put(nameof(Password), Password);
            k.Put(nameof(MQ), MQ);
        }
    }

}