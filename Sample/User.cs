using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// A user data object that is a principal.
    ///
    public class User : IData
    {
        public static readonly User Empty = new User();

        // admin types
        static readonly Opt<short> ADMIN = new Opt<short>
        {
            [0] = null,
            [2] = "本地管理员",
            [2] = "统一管理员",
        };

        internal string wx; // openid
        internal string nickname; // weixin nickname
        internal string name; // user name
        internal string tel;
        internal string password;
        internal string credential;
        internal string city; // 
        internal string addr;
        internal DateTime created;
        internal string shopid; // bound shop id
        internal short admin; // admin

        internal bool stored; // whether recorded in db

        public void ReadData(IDataInput i, int proj = 0)
        {
            if (proj.Prime())
            {
                i.Get(nameof(wx), ref wx);
            }
            i.Get(nameof(nickname), ref nickname);
            i.Get(nameof(name), ref name);
            i.Get(nameof(tel), ref tel);
            if (proj.Secret())
            {
                i.Get(nameof(password), ref password);
            }
            if (proj.Transf())
            {
                i.Get(nameof(credential), ref credential);
            }
            i.Get(nameof(city), ref city);
            i.Get(nameof(addr), ref addr);
            i.Get(nameof(created), ref created);
            if (proj.Late())
            {
                i.Get(nameof(shopid), ref shopid);
                i.Get(nameof(admin), ref admin);
            }
            if (proj.Ctrl())
            {
                i.Get(nameof(stored), ref stored);
            }
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            if (proj.Prime())
            {
                o.Put(nameof(wx), wx, label: "编号");
            }
            o.Put(nameof(nickname), nickname, label: "昵称");
            o.Put(nameof(name), name, label: "姓名");
            o.Put(nameof(tel), tel, label: "电话");
            if (proj.Secret())
            {
                o.Put(nameof(password), password, label: "密码");
            }
            if (proj.Transf())
            {
                o.Put(nameof(credential), credential);
            }
            o.Put(nameof(city), city);
            o.Put(nameof(addr), addr);
            o.Put(nameof(created), created);
            if (proj.Late())
            {
                o.Put(nameof(shopid), shopid, label: "供应点");
                o.Put(nameof(admin), admin, label: "管理员");
            }
            if (proj.Ctrl())
            {
                o.Put(nameof(stored), stored);
            }
        }

        public bool IsShop => shopid != null;
    }
}