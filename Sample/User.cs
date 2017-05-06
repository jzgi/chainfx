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
            INDB = 0x4000,
            WX = 0x0800,
            PERM = 0x0100,
            CREDENTIAL = 0x0040;

        public const short MANAGER = 1, ASSISTANT = 2, DELIVERER = 3;

        public readonly Opt<short> OPR = new Opt<short>
        {
            [MANAGER] = "经理",
            [ASSISTANT] = "助理",
            [DELIVERER] = "派送"
        };


        internal bool indb; // whether recorded in db

        internal string wx; // wexin openid
        internal string id; // optional unique id
        internal string credential;
        internal string name;
        internal string tel;
        internal string city; // default viewing city
        internal string distr;
        internal string addr;
        internal DateTime created;
        internal string oprat; // operator at shopid
        internal short opr; // 
        internal string sprat; // supervisor at city
        internal bool adm; // admininistrator


        public void ReadData(IDataInput i, short proj = 0)
        {
            if ((proj & INDB) == INDB)
            {
                i.Get(nameof(indb), ref indb);
            }
            if ((proj & WX) == WX)
            {
                i.Get(nameof(wx), ref wx);
            }
            i.Get(nameof(id), ref id);
            if ((proj & CREDENTIAL) == CREDENTIAL)
            {
                i.Get(nameof(credential), ref credential);
            }
            i.Get(nameof(name), ref name);
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(city), ref city);
            i.Get(nameof(distr), ref distr);
            i.Get(nameof(addr), ref addr);
            i.Get(nameof(created), ref created);
            if ((proj & PERM) == PERM)
            {
                i.Get(nameof(oprat), ref oprat);
                i.Get(nameof(opr), ref opr);
                i.Get(nameof(sprat), ref sprat);
                i.Get(nameof(adm), ref adm);
            }
        }

        public void WriteData<R>(IDataOutput<R> o, short proj = 0) where R : IDataOutput<R>
        {
            if ((proj & INDB) == INDB)
            {
                o.Put(nameof(indb), indb);
            }
            if ((proj & WX) == WX)
            {
                o.Put(nameof(wx), wx, label: "编号");
            }
            o.Put(nameof(id), id, label: "登录号");
            if ((proj & CREDENTIAL) == CREDENTIAL)
            {
                o.Put(nameof(credential), credential);
            }
            o.Put(nameof(name), name, label: "名称");
            o.Put(nameof(tel), tel, label: "电话");
            o.Put(nameof(city), city, label: "城市");
            o.Put(nameof(distr), distr, label: "区划");
            o.Put(nameof(addr), addr, label: "地址");
            o.Put(nameof(created), created);
            if ((proj & PERM) == PERM)
            {
                o.Put(nameof(opr), opr, label: "操作员");
                o.Put(nameof(oprat), oprat, label: "派送员");
                o.Put(nameof(sprat), sprat, label: "监管员");
                o.Put(nameof(adm), adm, label: "监管员");
            }
        }

        public bool IsShop => opr != 0;
    }
}