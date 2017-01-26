using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// A buyer data object.
    ///
    public class Buyer : IData
    {
        internal string wx; // weixin openid

        internal string name; // user name or weixin nickname

        internal string nickname; // weixin nickname

        internal string tel;

        internal DateTime orderon;

        internal decimal orderup;

        public void Load(ISource src, byte flags = 0)
        {
            src.Get(nameof(wx), ref wx);
            src.Get(nameof(nickname), ref nickname);
            src.Get(nameof(name), ref name);
            src.Get(nameof(tel), ref tel);
            src.Get(nameof(orderon), ref orderon);
            src.Get(nameof(orderup), ref orderup);
        }

        public void Dump<R>(ISink<R> snk, byte flags = 0) where R : ISink<R>
        {
            snk.Put(nameof(wx), wx);
            snk.Put(nameof(nickname), nickname);
            snk.Put(nameof(name), name);
            snk.Put(nameof(tel), tel);
            snk.Put(nameof(orderon), orderon);
            snk.Put(nameof(orderup), orderup);
        }
    }
}