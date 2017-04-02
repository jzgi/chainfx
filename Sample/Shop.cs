using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Shop : IData
    {
        public static readonly Shop Empty = new Shop();

        public Opt<short> Scopes = new Opt<short>
        {
            [0] = "本地",
            [1] = "全国"
        };

        internal string id;
        internal string name;
        internal string password;
        internal string credential;
        internal string tel;
        internal string mgr; // manager name
        internal string mgrwx; // manager weixin
        internal string city;
        internal string addr;
        internal double x;
        internal double y;
        internal short scope;
        internal ArraySegment<byte> icon;
        internal string descr;
        internal string lic;
        internal bool enabled;

        public void ReadData(IDataInput i, int proj = 0)
        {
            if (proj.Prime())
            {
                i.Get(nameof(id), ref id);
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
            if (proj.Late())
            {
                i.Get(nameof(mgrwx), ref mgrwx);
            }
            i.Get(nameof(city), ref city);
            i.Get(nameof(addr), ref addr);
            i.Get(nameof(x), ref x);
            i.Get(nameof(y), ref y);
            i.Get(nameof(scope), ref scope);
            i.Get(nameof(icon), ref icon);
            i.Get(nameof(descr), ref descr);
            if (proj.Immut())
            {
                i.Get(nameof(lic), ref lic);
            }
            if (proj.Power())
            {
                i.Get(nameof(enabled), ref enabled);
            }
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            if (proj.Prime())
            {
                o.Put(nameof(id), id, label: "编号", required: true);
            }
            o.Put(nameof(name), name, label: "名称");
            if (proj.Secret())
            {
                o.Put(nameof(password), password, label: "密码", max: 20);
            }
            if (proj.Transf())
            {
                o.Put(nameof(credential), credential);
            }
            o.Put(nameof(tel), tel, label: "电话", max: 11);
            if (proj.Late())
            {
                o.Put(nameof(mgrwx), mgrwx);
            }
            o.Put(nameof(city), city, label: "城市", max: 10);
            o.Put(nameof(addr), addr, label: "地址", max: 10);
            o.Put(nameof(x), x);
            o.Put(nameof(y), y);
            o.Put(nameof(scope), scope, label: "覆盖", opt: Scopes);
            o.Put(nameof(icon), icon);
            o.Put(nameof(descr), descr, label: "简语");
            if (proj.Immut())
            {
                o.Put(nameof(lic), lic, label: "工商登记");
            }
            if (proj.Power())
            {
                o.Put(nameof(enabled), enabled, label: "运行中", opt: (b) => b ? "是" : "否");
            }
        }
    }
}