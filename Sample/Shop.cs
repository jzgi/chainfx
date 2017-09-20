using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Shop : IData
    {
        public static readonly Shop Empty = new Shop();

        public const int
            ID = 0x0001,
            BASIC = 0x0002,
            BASIC_ICON = 0x006,
            ADMIN = 0x0020,
            ADMIN_WX = 0x0040;

        public static Map<short, string> STATUS = new Map<short, string>
        {
            [0] = "停业",
            [1] = "休假中",
            [2] = "营业中"
        };

        internal string id;
        internal string name;
        internal string tel;
        internal string city;
        internal string addr;
        internal double x;
        internal double y;
        internal string lic;
        internal DateTime created;
        internal string mgrid; // set by admin
        internal string mgrwx;
        internal string mgr;

        internal Target[] targets;
        
        internal short status;

        public void Read(IDataInput i, int proj = 0x00ff)
        {
            if ((proj & ID) == ID)
            {
                i.Get(nameof(id), ref id);
            }
            if ((proj & BASIC) == BASIC)
            {
                i.Get(nameof(name), ref name);
                i.Get(nameof(tel), ref tel);
                i.Get(nameof(city), ref city);
                i.Get(nameof(addr), ref addr);
                i.Get(nameof(x), ref x);
                i.Get(nameof(y), ref y);
                i.Get(nameof(lic), ref lic);
                i.Get(nameof(created), ref created);
            }
            if ((proj & ADMIN) == ADMIN)
            {
                i.Get(nameof(mgrid), ref mgrid);
                if ((proj & ADMIN_WX) == ADMIN_WX)
                {
                    i.Get(nameof(mgrwx), ref mgrwx);
                }
                i.Get(nameof(mgr), ref mgr);
            }
            i.Get(nameof(status), ref status);
        }

        public void Write<R>(IDataOutput<R> o, int proj = 0x00ff) where R : IDataOutput<R>
        {
            if ((proj & ID) == ID)
            {
                o.Put(nameof(id), id, "编号");
            }
            if ((proj & BASIC) == BASIC)
            {
                o.Put(nameof(name), name, "名称");
                o.Put(nameof(tel), tel, "电话");
                o.Put(nameof(city), city, "城市");
                o.Put(nameof(addr), addr, "地址");
                o.Put(nameof(x), x, "X");
                o.Put(nameof(y), y, "Y");
                o.Put(nameof(lic), lic, "工商登记");
                o.Put(nameof(created), created, "创建时间");
            }
            if ((proj & ADMIN) == ADMIN)
            {
                o.Group("经理");
                o.Put(nameof(mgrid), mgrid);
                if ((proj & ADMIN_WX) == ADMIN_WX)
                {
                    o.Put(nameof(mgrwx), mgrwx);
                }
                o.Put(nameof(mgr), mgr);
                o.UnGroup();
            }
            o.Put(nameof(status), status, "状态", STATUS);
        }
    }


    public struct Target
    {
        internal string name;

        internal double x0, y0, x1, y1;
    }
}