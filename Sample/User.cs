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
        static readonly Map<short> ADMIN = new Map<short>
        {
            [0] = null,
            [2] = "分管员",
            [2] = "统管员",
        };

        internal string wx; // openid
        internal string nickname; // weixin nickname
        internal string name; // user name
        internal string tel;
        internal string password;
        internal string credential;
        internal string city; // 
        internal DateTime created;
        internal string shopid; // bound shop id
        internal short admin; // admin
        internal decimal addup; // orders addup

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
            i.Get(nameof(created), ref created);
            if (proj.Late())
            {
                i.Get(nameof(shopid), ref shopid);
                i.Get(nameof(admin), ref admin);
                i.Get(nameof(addup), ref addup);
            }
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            if (proj.Prime())
            {
                o.Put(nameof(wx), wx, Label: "编号");
            }
            o.Put(nameof(nickname), nickname, Label: "昵称");
            o.Put(nameof(name), name, Label: "姓名");
            o.Put(nameof(tel), tel, Label: "电话");
            if (proj.Secret())
            {
                o.Put(nameof(password), password, Label: "密码");
            }
            if (proj.Transf())
            {
                o.Put(nameof(credential), credential);
            }
            o.Put(nameof(city), city);
            o.Put(nameof(created), created);
            if (proj.Late())
            {
                o.Put(nameof(shopid), shopid, Label: "供应点");
                o.Put(nameof(admin), admin, Label: "管理员");
                o.Put(nameof(addup), addup);
            }
        }

        public bool IsShop => shopid != null;
    }
}