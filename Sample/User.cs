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

        internal bool stored; // whether recorded in db

        internal string wx; // openid
        internal string name;
        internal string credential;
        internal string tel;
        internal string city; //
        internal string distr;
        internal string addr;
        internal DateTime created;
        internal string oprat; // operator at shopid
        internal string dvrat; // deliverer at shopid
        internal string mgrat; // manager at city
        internal bool adm; // manager of city

        public void ReadData(IDataInput i, int proj = 0)
        {
            if (proj.Ctrl())
            {
                i.Get(nameof(stored), ref stored);
            }

            if (proj.Prime())
            {
                i.Get(nameof(wx), ref wx);
            }
            i.Get(nameof(name), ref name);
            if (proj.Transf())
            {
                i.Get(nameof(credential), ref credential);
            }
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(city), ref city);
            i.Get(nameof(distr), ref distr);
            i.Get(nameof(addr), ref addr);
            i.Get(nameof(created), ref created);
            if (proj.Late())
            {
                i.Get(nameof(oprat), ref oprat);
                i.Get(nameof(dvrat), ref dvrat);
                i.Get(nameof(mgrat), ref mgrat);
                i.Get(nameof(adm), ref adm);
            }
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            if (proj.Ctrl())
            {
                o.Put(nameof(stored), stored);
            }

            if (proj.Prime())
            {
                o.Put(nameof(wx), wx, label: "编号");
            }
            o.Put(nameof(name), name, label: "名称");
            if (proj.Transf())
            {
                o.Put(nameof(credential), credential);
            }
            o.Put(nameof(tel), tel, label: "电话");
            o.Put(nameof(city), city, label: "城市");
            o.Put(nameof(distr), distr, label: "区县");
            o.Put(nameof(addr), addr, label: "地址");
            o.Put(nameof(created), created);
            if (proj.Late())
            {
                o.Put(nameof(oprat), oprat, label: "操作员");
                o.Put(nameof(dvrat), dvrat, label: "派送员");
                o.Put(nameof(mgrat), mgrat, label: "监管员");
                o.Put(nameof(addr), addr, label: "监管员");
            }
        }

        public bool IsShop => oprat != null;
    }
}