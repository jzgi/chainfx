using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// A buyer data object.
    ///
    public class Buyer : IData
    {
        internal string openid; // weixin openid

        internal string nickname; // weixin nickname

        internal string name; // wechat id

        internal string tel;

        internal DateTime last;

        internal decimal addup;

        public void Load(ISource s, byte flags = 0)
        {
            s.Get(nameof(openid), ref openid);
            s.Get(nameof(nickname), ref nickname);
            s.Get(nameof(name), ref name);
            s.Get(nameof(tel), ref tel);
            s.Get(nameof(last), ref last);
            s.Get(nameof(addup), ref addup);
        }

        public void Dump<R>(ISink<R> s, byte flags = 0) where R : ISink<R>
        {
            s.Put(nameof(openid), openid);
            s.Put(nameof(nickname), nickname);
            s.Put(nameof(name), name);
            s.Put(nameof(tel), tel);
            s.Put(nameof(last), last);
            s.Put(nameof(addup), addup);
        }
    }
}