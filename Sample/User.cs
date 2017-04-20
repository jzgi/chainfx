using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// A user data object that is a principal.
    ///
    public class User : IData
    {
        public const short MGR = 1, DVR = 2;

        public static readonly User Empty = new User();

        // operator
        static readonly Opt<short> OPR = new Opt<short>
        {
            [0] = null,
            [MGR] = "经理",
            [DVR] = "派送员",
            [MGR | DVR] = "经理兼派送员"
        };

        // administrator
        static readonly Opt<short> ADM = new Opt<short>
        {
            [0] = null,
            [1] = "本地监管员",
            [2] = "系统监管员",
        };

        internal bool stored; // whether recorded in db

        internal string wx; // openid
        internal string name;
        internal string password;
        internal string credential;
        internal string tel;
        internal string city; //
        internal string distr;
        internal string addr;
        internal DateTime created;
        internal string oprat; // operator of shopid
        internal string dvrat; // deliverer of shopid
        internal string mgrat; // manager of city

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
            if (proj.Secret())
            {
                i.Get(nameof(password), ref password);
            }
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
            if (proj.Secret())
            {
                o.Put(nameof(password), password, label: "密码");
            }
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
            }
        }

        public bool IsShop => oprat != null;
    }
}