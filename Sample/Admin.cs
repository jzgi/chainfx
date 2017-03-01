using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// A user that is a platform internal worker.
    ///
    public class Admin : IData
    {
        internal string id; // weixin openid

        internal string name;

        internal string credential;

        internal int roles;

        internal string tel;

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(name), ref name);
            i.Get(nameof(credential), ref credential);
            i.Get(nameof(roles), ref roles);
            i.Get(nameof(tel), ref tel);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(name), name);
            o.Put(nameof(credential), credential);
            o.Put(nameof(roles), roles);
            o.Put(nameof(tel), tel);
        }

        public Token ToToken()
        {
            return new Token()
            {
                key = id,
                name = name,
                roles = roles,
                extra = null
            };
        }
    }
}