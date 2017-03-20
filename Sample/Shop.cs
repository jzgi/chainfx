using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Shop : IData
    {
        public static readonly Shop Empty = new Shop();

        public Map<short> Scopes = new Map<short>
        {
            [1] = "方圆三十公里",
            [2] = "方圆一百公里",
            [3] = "方圆一千公里"
        };

        internal string id;
        internal string name;
        internal string password;
        internal string credential;
        internal string tel;
        internal string mgrid;
        internal string province;
        internal string city;
        internal double x;
        internal double y;
        internal short scope;
        internal ArraySegment<byte> icon;
        internal string descr;
        internal string license;
        internal bool enabled;

        public void ReadData(IDataInput i, int proj = 0)
        {
            if (proj.Ctrl())
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
            i.Get(nameof(mgrid), ref mgrid);
            i.Get(nameof(province), ref province);
            i.Get(nameof(city), ref city);
            i.Get(nameof(x), ref x);
            i.Get(nameof(y), ref y);
            i.Get(nameof(scope), ref scope);
            i.Get(nameof(icon), ref icon);
            i.Get(nameof(descr), ref descr);
            if (proj.Ctrl())
            {
                i.Get(nameof(license), ref license);
                i.Get(nameof(enabled), ref enabled);
            }
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            if (proj.Ctrl())
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
            o.Put(nameof(mgrid), mgrid, Label: "管理员编号");
            o.Put(nameof(province), province);
            o.Put(nameof(city), city, Label: "城市", Max: 10);
            o.Put(nameof(x), x);
            o.Put(nameof(y), y);
            o.Put(nameof(scope), scope, Label: "覆盖", Opt: Scopes);
            o.Put(nameof(icon), icon);
            o.Put(nameof(descr), descr, Label: "简语");
            if (proj.Ctrl())
            {
                o.Put(nameof(license), license, Label: "工商登记");
                o.Put(nameof(enabled), enabled, Label: "运行中", Opt: (b) => b ? "是" : "否");
            }
        }
    }
}