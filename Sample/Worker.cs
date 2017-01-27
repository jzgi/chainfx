using System;
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

        internal DateTime orderon;

        internal decimal orderup;

        public void Load(ISource src, byte flags = 0)
        {
            src.Get(nameof(id), ref id);
            src.Get(nameof(name), ref name);
            src.Get(nameof(roles), ref roles);
            src.Get(nameof(tel), ref tel);
            src.Get(nameof(orderon), ref orderon);
            src.Get(nameof(orderup), ref orderup);
        }

        public void Dump<R>(ISink<R> snk, byte flags = 0) where R : ISink<R>
        {
            snk.Put(nameof(id), id);
            snk.Put(nameof(name), name);
            snk.Put(nameof(roles), roles);
            snk.Put(nameof(tel), tel);
            snk.Put(nameof(orderon), orderon);
            snk.Put(nameof(orderup), orderup);
        }

        public Token ToToken()
        {
            return new Token()
            {
                key = id,
                name = name,
                subtype = 1,
                roles = roles
            };
        }
    }
}