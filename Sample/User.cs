using System;
using System.Collections.Generic;
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

            // non-data or for control
            CTRL = 0x4000,

            // primary or key
            PRIME = 0x0800,

            // auto generated or with default
            AUTO = 0x0400,

            // binary
            BIN = 0x0200,

            // late-handled
            LATE = 0x0100,

            // many
            DETAIL = 0x0080,

            // transform or digest
            TRANSF = 0x0040,

            // secret or protected
            SECRET = 0x0020,

            // need authority
            POWER = 0x0010,

            // frozen or immutable
            IMMUT = 0x0008;


        internal bool stored; // whether recorded in db

        internal string wx; // openid
        internal string name;
        internal string city; // default viewing city
        internal string tel;
        internal string credential;
        internal DateTime created;
        internal string oprat; // operator at shopid
        internal string dvrat; // deliverer at shopid
        internal string mgrat; // manager at city
        internal bool adm; // administrator

        internal Address[] addrs;


        public void ReadData(IDataInput i, short proj = 0)
        {
            if ((proj & CTRL) == CTRL)
            {
                i.Get(nameof(stored), ref stored);
            }

            if ((proj & PRIME) == PRIME)
            {
                i.Get(nameof(wx), ref wx);
            }
            i.Get(nameof(name), ref name);
            if ((proj & TRANSF) == TRANSF)
            {
                i.Get(nameof(credential), ref credential);
            }
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(city), ref city);
            i.Get(nameof(created), ref created);
            if ((proj & LATE) == LATE)
            {
                i.Get(nameof(oprat), ref oprat);
                i.Get(nameof(dvrat), ref dvrat);
                i.Get(nameof(mgrat), ref mgrat);
                i.Get(nameof(adm), ref adm);
            }
            i.Get(nameof(addrs), ref addrs);
        }

        public void WriteData<R>(IDataOutput<R> o, short proj = 0) where R : IDataOutput<R>
        {
            if ((proj & CTRL) == CTRL)
            {
                o.Put(nameof(stored), stored);
            }

            if ((proj & PRIME) == PRIME)
            {
                o.Put(nameof(wx), wx, label: "编号");
            }
            o.Put(nameof(name), name, label: "名称");
            if ((proj & TRANSF) == TRANSF)
            {
                o.Put(nameof(credential), credential);
            }
            o.Put(nameof(tel), tel, label: "电话");
            o.Put(nameof(city), city, label: "城市");
            o.Put(nameof(created), created);
            if ((proj & LATE) == LATE)
            {
                o.Put(nameof(oprat), oprat, label: "操作员");
                o.Put(nameof(dvrat), dvrat, label: "派送员");
                o.Put(nameof(mgrat), mgrat, label: "监管员");
                o.Put(nameof(adm), adm, label: "监管员");
            }
            o.Put(nameof(addrs), addrs, label: "地址");
        }

        public bool IsShop => oprat != null;
    }

    ///
    ///
    public class Address : IData
    {
        public static readonly Item Empty = new Item();

        internal string tel;
        internal string city;
        internal string distr;
        internal string addr;

        public void ReadData(IDataInput i, short proj = 0)
        {
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(city), ref city);
            i.Get(nameof(distr), ref distr);
            i.Get(nameof(addr), ref addr);
        }

        public void WriteData<R>(IDataOutput<R> o, short proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(tel), tel);
            o.Put(nameof(city), city);
            o.Put(nameof(distr), distr);
            o.Put(nameof(addr), addr);
        }
    }
}