namespace Greatbone.Core
{
    public class DbConfig : IPersist
    {
        public string Host;

        public int Port;

        public string Username;

        public string Password;

        public bool MQ;

        public void Load(ISource sc)
        {
            sc.Got(nameof(Host), ref Host);
            sc.Got(nameof(Port), ref Port);
            sc.Got(nameof(Username), ref Username);
            sc.Got(nameof(Password), ref Password);
            sc.Got(nameof(MQ), ref MQ);
        }

        public void Save<R>(ISink<R> k) where R : ISink<R>
        {
            k.Put(nameof(Host), Host);
            k.Put(nameof(Port), Port);
            k.Put(nameof(Username), Username);
            k.Put(nameof(Password), Password);
            k.Put(nameof(MQ), MQ);
        }
    }

}