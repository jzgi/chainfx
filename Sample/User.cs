using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// A buyer data object.
    ///
    public class User : IData
    {
        internal string wx; // weixin openid

        internal string name; // user name or weixin nickname

        internal string nickname; // weixin nickname

        internal string tel;

        internal DateTime orderon; // last order time

        internal decimal orderup; // accumulative addup

        public void ReadData(IDataInput i, ushort proj = 0)
        {
            i.Get(nameof(wx), ref wx);
            i.Get(nameof(nickname), ref nickname);
            i.Get(nameof(name), ref name);
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(orderon), ref orderon);
            i.Get(nameof(orderup), ref orderup);
        }

        public void WriteData<R>(IDataOutput<R> o, ushort proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(wx), wx);
            o.Put(nameof(nickname), nickname);
            o.Put(nameof(name), name);
            o.Put(nameof(tel), tel);
            o.Put(nameof(orderon), orderon);
            o.Put(nameof(orderup), orderup);
        }

        public Token ToToken()
        {
            return new Token()
            {
                key = wx,
                name = name,
                role = 3
            };
        }
    }
}