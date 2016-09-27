using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public struct Fame : IDat
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

        public void From(IInput i)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(name), ref name);
            i.Get(nameof(quote), ref quote);
            i.Get(nameof(sex), ref sex);
            i.Get(nameof(icon), ref icon);
            i.Get(nameof(birthday), ref birthday);
            i.Get(nameof(qq), ref qq);
            i.Get(nameof(wechat), ref wechat);
            i.Get(nameof(email), ref email);
            i.Get(nameof(city), ref city);
            i.Get(nameof(rank), ref rank);
            i.Get(nameof(height), ref height);
            i.Get(nameof(weight), ref weight);
            i.Get(nameof(bust), ref bust);
            i.Get(nameof(waist), ref waist);
            i.Get(nameof(hip), ref hip);
            i.Get(nameof(cup), ref cup);
            i.Get(nameof(styles), ref styles);
            i.Get(nameof(skills), ref skills);
            i.Get(nameof(remark), ref remark);
            i.Get(nameof(sites), ref sites);
            i.Get(nameof(friends), ref friends);
            i.Get(nameof(awards), ref awards);
        }

        public void To(IOutput w)
        {
            w.Put(nameof(id), id);
            w.Put(nameof(name), name);
            w.Put(nameof(quote), quote);
            w.Put(nameof(sex), sex);
            w.Put(nameof(icon), icon);
            w.Put(nameof(birthday), birthday);
            w.Put(nameof(qq), qq);
            w.Put(nameof(wechat), wechat);
            w.Put(nameof(email), email);
            w.Put(nameof(city), city);
            w.Put(nameof(rank), rank);
            w.Put(nameof(height), height);
            w.Put(nameof(weight), weight);
            w.Put(nameof(bust), bust);
            w.Put(nameof(waist), waist);
            w.Put(nameof(hip), hip);
            w.Put(nameof(cup), cup);
            w.Put(nameof(styles), styles);
            w.Put(nameof(skills), skills);
            w.Put(nameof(remark), remark);
            w.Put(nameof(sites), sites);
            w.Put(nameof(friends), friends);
            w.Put(nameof(awards), awards);
        }
    }

    public struct Item
    {
        internal char[] uid;

        internal string url;

        internal string desc;

        public void From(IInput r)
        {
            r.Get(nameof(uid), ref uid);
            r.Get(nameof(url), ref url);
            r.Get(nameof(desc), ref desc);
        }

        public void To(IOutput w)
        {
            w.Put(nameof(uid), uid);
            w.Put(nameof(url), url);
            w.Put(nameof(desc), desc);
        }
    }
}