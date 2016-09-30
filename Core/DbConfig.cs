namespace Greatbone.Core
{
    public class DbConfig : IData
    {
        public string Host;

        public int Port;

        public string Username;

        public string Password;

        public bool MQ;

        public void In(IDataIn r)
        {
            r.Get(nameof(Host), ref Host);
            r.Get(nameof(Port), ref Port);
            r.Get(nameof(Username), ref Username);
            r.Get(nameof(Password), ref Password);
            r.Get(nameof(MQ), ref MQ);
        }

        public void Out<R>(IDataOut<R> o) where R : IDataOut<R>
        {
            o.Put(nameof(Host), Host);
            o.Put(nameof(Port), Port);
            o.Put(nameof(Username), Username);
            o.Put(nameof(Password), Password);
            o.Put(nameof(MQ), MQ);
        }
    }

}