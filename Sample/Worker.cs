using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// A user that is a platform internal worker.
    ///
    public class Worker : IData
    {
        internal string id; // weixin openid

        internal string name; // user name or weixin nickname

        internal short roles;

        internal string tel;

        public void ReadData(IDataInput i, ushort proj = 0)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(name), ref name);
            i.Get(nameof(roles), ref roles);
            i.Get(nameof(tel), ref tel);
        }

        public void WriteData<R>(IDataOutput<R> o, ushort proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(name), name);
            o.Put(nameof(roles), roles);
            o.Put(nameof(tel), tel);
        }

        public Token ToToken()
        {
            return new Token()
            {
                key = id,
                name = name,
                role = 1,
                extra = null
            };
        }
    }
}