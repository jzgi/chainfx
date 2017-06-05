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

        public const short
            WX = 2,
            CREATTED = 2,
            BACKEND = 0x00f0, // inclusive
            CREDENTIAL = 0x0010,
            PERM = 0x0100;

        public const short MANAGER = 7, AID = 3;

        public static readonly Opt<short> OPR = new Opt<short>
        {
            [MANAGER] = "经理",
            [AID] = "助理"
        };


        internal string wx; // wexin openid

        internal string nickname;
        internal string city; // default viewing city
        internal string distr;
        internal string addr;
        internal string tel;
        internal DateTime created;

        internal string name;
        internal string id; // optional unique id
        internal string credential;
        internal string oprat; // operator at shopid
        internal short opr; // 
        internal string sprat; // supervisor at city
        internal bool adm; // admininistrator


        public void ReadData(IDataInput i, ushort proj = 0)
        {
            if ((proj & WX) == WX)
            {
                i.Get(nameof(wx), ref wx);
            }
            i.Get(nameof(nickname), ref nickname);
            i.Get(nameof(city), ref city);
            i.Get(nameof(distr), ref distr);
            i.Get(nameof(addr), ref addr);
            i.Get(nameof(tel), ref tel);

            if ((proj & CREATTED) == CREATTED)
            {
                i.Get(nameof(created), ref created);
            }

            if ((proj & BACKEND) != 0) // inclusive
            {
                i.Get(nameof(id), ref id);
                i.Get(nameof(name), ref name);
                if ((proj & CREDENTIAL) == CREDENTIAL)
                {
                    i.Get(nameof(credential), ref credential);
                }
            }
            if ((proj & PERM) == PERM)
            {
                i.Get(nameof(oprat), ref oprat);
                i.Get(nameof(opr), ref opr);
                i.Get(nameof(sprat), ref sprat);
                i.Get(nameof(adm), ref adm);
            }
        }

        public void WriteData<R>(IDataOutput<R> o, ushort proj = 0) where R : IDataOutput<R>
        {
            if ((proj & WX) == WX)
            {
                o.Put(nameof(wx), wx);
            }
            o.Put(nameof(nickname), nickname, "用户名称");
            o.Put(nameof(city), city, "城市");
            o.Put(nameof(distr), distr, "区划");
            o.Put(nameof(addr), addr, "街道/地址");
            o.Put(nameof(tel), tel, "电话");

            if ((proj & CREATTED) == CREATTED)
            {
                o.Put(nameof(created), created);
            }
            if ((proj & BACKEND) != 0)
            {
                o.Put(nameof(id), id, "后台帐号");
                o.Put(nameof(name), name, "真实姓名");
                if ((proj & CREDENTIAL) == CREDENTIAL)
                {
                    o.Put(nameof(credential), credential);
                }
            }
            if ((proj & PERM) == PERM)
            {
                o.Put(nameof(oprat), oprat, "所在商家");
                o.Put(nameof(opr), opr, "操作权限", OPR);
                o.Put(nameof(sprat), sprat, "区域监督");
                o.Put(nameof(adm), adm, "平台管理");
            }
        }
    }
}