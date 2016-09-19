using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Fame : ISerial
    {
        internal char[] id;
        internal string name;
        internal string quote;
        internal bool sex;
        internal byte[] icon;
        internal DateTime birthday;
        internal string qq;
        internal string wechat;
        internal string email;
        internal string city;
        internal short rank;
        internal short height;
        internal short weight;
        internal short bust;
        internal short waist;
        internal short hip;
        internal short cup;
        internal short styles;
        internal short skills;
        internal short remark;
        internal List<Item> sites;
        internal List<Item> friends;
        internal List<Item> awards;

        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(id), out id);
            r.Read(nameof(name), out name);
            r.Read(nameof(quote), out quote);
            r.Read(nameof(sex), out sex);
            r.Read(nameof(icon), out icon);
            r.Read(nameof(birthday), out birthday);
            r.Read(nameof(qq), out qq);
            r.Read(nameof(wechat), out wechat);
            r.Read(nameof(email), out email);
            r.Read(nameof(city), out city);
            r.Read(nameof(rank), out rank);
            r.Read(nameof(height), out height);
            r.Read(nameof(weight), out weight);
            r.Read(nameof(bust), out bust);
            r.Read(nameof(waist), out waist);
            r.Read(nameof(hip), out hip);
            r.Read(nameof(cup), out cup);
            r.Read(nameof(styles), out styles);
            r.Read(nameof(skills), out skills);
            r.Read(nameof(remark), out remark);
            r.Read(nameof(sites), out sites);
            r.Read(nameof(friends), out friends);
            r.Read(nameof(awards), out awards);
        }

        public void WriteTo(ISerialWriter w)
        {
            w.Write(nameof(id), id);
            w.Write(nameof(name), name);
            w.Write(nameof(quote), quote);
            w.Write(nameof(sex), sex);
            w.Write(nameof(icon), icon);
            w.Write(nameof(birthday), birthday);
            w.Write(nameof(qq), qq);
            w.Write(nameof(wechat), wechat);
            w.Write(nameof(email), email);
            w.Write(nameof(city), city);
            w.Write(nameof(rank), rank);
            w.Write(nameof(height), height);
            w.Write(nameof(weight), weight);
            w.Write(nameof(bust), bust);
            w.Write(nameof(waist), waist);
            w.Write(nameof(hip), hip);
            w.Write(nameof(cup), cup);
            w.Write(nameof(styles), styles);
            w.Write(nameof(skills), skills);
            w.Write(nameof(remark), remark);
            w.Write(nameof(sites), sites);
            w.Write(nameof(friends), friends);
            w.Write(nameof(awards), awards);
        }
    }

    public struct Item : ISerial
    {
        internal char[] uid;

        internal string url;

        internal string desc;

        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(uid), out uid);
            r.Read(nameof(url), out url);
            r.Read(nameof(desc), out desc);
        }

        public void WriteTo(ISerialWriter w)
        {
            w.Write(nameof(uid), uid);
            w.Write(nameof(url), url);
            w.Write(nameof(desc), desc);
        }
    }
}