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
            r.Read(nameof(id), ref id);
            r.Read(nameof(name), ref name);
            r.Read(nameof(quote), ref quote);
            r.Read(nameof(sex), ref sex);
            r.Read(nameof(icon), ref icon);
            r.Read(nameof(birthday), ref birthday);
            r.Read(nameof(qq), ref qq);
            r.Read(nameof(wechat), ref wechat);
            r.Read(nameof(email), ref email);
            r.Read(nameof(city), ref city);
            r.Read(nameof(rank), ref rank);
            r.Read(nameof(height), ref height);
            r.Read(nameof(weight), ref weight);
            r.Read(nameof(bust), ref bust);
            r.Read(nameof(waist), ref waist);
            r.Read(nameof(hip), ref hip);
            r.Read(nameof(cup), ref cup);
            r.Read(nameof(styles), ref styles);
            r.Read(nameof(skills), ref skills);
            r.Read(nameof(remark), ref remark);
            r.Read(nameof(sites), ref sites);
            r.Read(nameof(friends), ref friends);
            r.Read(nameof(awards), ref awards);
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
            r.Read(nameof(uid), ref uid);
            r.Read(nameof(url), ref url);
            r.Read(nameof(desc), ref desc);
        }

        public void WriteTo(ISerialWriter w)
        {
            w.Write(nameof(uid), uid);
            w.Write(nameof(url), url);
            w.Write(nameof(desc), desc);
        }
    }
}