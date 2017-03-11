using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// A shop data object.
    ///
    public class Shop : IData
    {
        public static readonly Shop Empty = new Shop();

        public static readonly Set<short> STATUS = new Set<short>
        {
            [0] = "禁用",
            [1] = "营业",
            [2] = "休息",
        };

        internal string id; // platform shop id
        internal string name;
        internal string password;
        internal string credential;
        internal string tel;
        internal string wx;
        internal string city;
        internal double x;
        internal double y;
        internal ArraySegment<byte> icon;
        internal string descr;
        internal string license;
        internal short status; // -1 dismissed, 0 closed, 1 open

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(name), ref name);
            if (proj.Kept())
            {
                i.Get(nameof(credential), ref credential);
            }
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(wx), ref wx);
            i.Get(nameof(city), ref city);
            i.Get(nameof(x), ref x);
            i.Get(nameof(y), ref y);
            i.Get(nameof(icon), ref icon);
            i.Get(nameof(descr), ref descr);
            i.Get(nameof(license), ref license);
            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(name), name);
            if (proj.Kept())
            {
                o.Put(nameof(credential), credential);
            }
            o.Put(nameof(tel), tel);
            o.Put(nameof(x), x);
            o.Put(nameof(y), y);
            o.Put(nameof(icon), icon);
            o.Put(nameof(city), city);
            o.Put(nameof(wx), wx);
            o.Put(nameof(descr), descr);
            o.Put(nameof(license), license);
            o.Put(nameof(status), status, Opt: STATUS);
        }
    }
}