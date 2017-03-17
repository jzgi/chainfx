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
            if (proj.Ctrl())
            {
                i.Get(nameof(id), ref id);
            }
            if (proj.Secret())
            {
                i.Get(nameof(password), ref password);
            }
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
            if (proj.Ctrl())
            {
                o.Put(nameof(id), id, Label: "编号");
            }
            o.Put(nameof(wx), wx);
            if (proj.Secret())
            {
                o.Put(nameof(password), password, Label: "密码");
            }
            if (proj.Code())
            {
                o.Put(nameof(credential), credential);
            }
            o.Put(nameof(nickname), nickname, Label: "昵称");
            o.Put(nameof(name), name, Label: "姓名");
            o.Put(nameof(tel), tel, Label: "电话");
            o.Put(nameof(shopid), shopid, Label: "供应点");
            o.Put(nameof(jobs), jobs, Label: "职能");

            o.Put(nameof(ordered), ordered);
            o.Put(nameof(addup), addup);
        }

        public bool IsAdmin => jobs != 0;

        public bool IsShop => shopid != null;
    }
}