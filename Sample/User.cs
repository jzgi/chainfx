using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// A user principal object.
    ///
    public class User : IData
    {
        public static readonly User Empty = new User();

        // jobs
        public const short GLOBALADMIN = 3, LOCALADMIN = 1;

        internal string id; // openid
        internal string nickname; // weixin nickname
        internal string name; // user name
        internal string tel;
        internal string password;
        internal string credential;
        internal string province; // province
        internal string city; // 
        internal string shopid; // bound shop id
        internal bool admin; // local administrator
        internal bool sa; // system administrator
        internal DateTime created;
        internal decimal addup; // orders addup
        internal bool disabled;

        public void ReadData(IDataInput i, int proj = 0)
        {
            if (proj.Ctrl())
            {
                i.Get(nameof(id), ref id);
            }
            i.Get(nameof(nickname), ref nickname);
            i.Get(nameof(name), ref name);
            i.Get(nameof(tel), ref tel);
            if (proj.Transient())
            {
                i.Get(nameof(password), ref password);
            }
            if (proj.Code())
            {
                i.Get(nameof(credential), ref credential);
            }
            i.Get(nameof(province), ref province);
            i.Get(nameof(city), ref city);
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(admin), ref admin);
            i.Get(nameof(created), ref created);
            i.Get(nameof(addup), ref addup);
            i.Get(nameof(disabled), ref disabled);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            if (proj.Ctrl())
            {
                o.Put(nameof(id), id, Label: "编号");
            }
            o.Put(nameof(nickname), nickname, Label: "昵称");
            o.Put(nameof(name), name, Label: "姓名");
            o.Put(nameof(tel), tel, Label: "电话");
            if (proj.Transient())
            {
                o.Put(nameof(password), password, Label: "密码");
            }
            if (proj.Code())
            {
                o.Put(nameof(credential), credential);
            }
            o.Put(nameof(province), admin, Label: "省份");
            o.Put(nameof(city), city);
            o.Put(nameof(shopid), shopid, Label: "供应点");
            o.Put(nameof(admin), admin, Label: "管理员");
            o.Put(nameof(created), created);
            o.Put(nameof(addup), addup);
            o.Put(nameof(disabled), disabled);
        }

        public bool IsAdmin => admin;

        public bool IsShop => shopid != null;
    }
}