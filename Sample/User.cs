using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// A user data object.
    ///
    public class User : IData
    {
        public static readonly User Empty = new User();

        // jobs
        public const short MARKETG = 0x11, ACCOUNTG = 0x12, CUSTOMER_SVC = 0x14;

        internal string id; // tel initially
        internal string password;
        internal string credential;
        internal string wx; // weixin openid
        internal string nickname; // weixin nickname
        internal string name; // user name
        internal string tel;
        internal string shopid; // bound shop id
        internal short jobs; // platform jobs

        internal DateTime ordered; // lasttime ordered
        internal decimal addup; // orders addup

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(id), ref id);
            if (proj.Code())
            {
                i.Get(nameof(credential), ref credential);
            }
            i.Get(nameof(wx), ref wx);
            i.Get(nameof(nickname), ref nickname);
            i.Get(nameof(name), ref name);
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(jobs), ref jobs);

            i.Get(nameof(ordered), ref ordered);
            i.Get(nameof(addup), ref addup);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(wx), wx);
            o.Put(nameof(nickname), nickname);
            o.Put(nameof(name), name);
            o.Put(nameof(tel), tel);
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(jobs), jobs);

            o.Put(nameof(ordered), ordered);
            o.Put(nameof(addup), addup);
        }

        public bool IsAdmin => false;

        public bool IsShop => false;

    }
}