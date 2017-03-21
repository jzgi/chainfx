using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Shop : IData
    {
        public static readonly Shop Empty = new Shop();

        public Map<short> Scopes = new Map<short>
        {
            [1] = "三十公里",
            [2] = "一百公里",
            [3] = "一千公里"
        };

        internal string id;
        internal string name;
        internal string password;
        internal string credential;
        internal string tel;
        internal string mgr; // manager name
        internal string mgrwx; // manager weixin
        internal string city;
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
                o.Put(nameof(id), id, Label: "编号", Required: true);
            }
            o.Put(nameof(name), name, Label: "名称");
            if (proj.Secret())
            {
                o.Put(nameof(password), password, Label: "密码", Max: 20);
            }
            if (proj.Transf())
            {
                o.Put(nameof(credential), credential);
            }
            o.Put(nameof(tel), tel, Label: "电话", Max: 11);
            if (proj.Late())
            {
                o.Put(nameof(mgrwx), mgrwx);
            }
            o.Put(nameof(city), city, Label: "城市", Max: 10);
            o.Put(nameof(x), x);
            o.Put(nameof(y), y);
            o.Put(nameof(scope), scope, Label: "覆盖", Opt: Scopes);
            o.Put(nameof(icon), icon);
            o.Put(nameof(descr), descr, Label: "简语");
            if (proj.Immut())
            {
                o.Put(nameof(lic), lic, Label: "工商登记");
            }
            if (proj.Power())
            {
                o.Put(nameof(enabled), enabled, Label: "运行中", Opt: (b) => b ? "是" : "否");
            }
        }
    }
}