using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{

    public struct Fame : IPersist
    {
        internal string id;
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

        public void Load(ISource sc, int x)
        {
            sc.Get(nameof(id), ref id);
            sc.Get(nameof(name), ref name);
            sc.Get(nameof(quote), ref quote);
            sc.Get(nameof(sex), ref sex);
            sc.Get(nameof(icon), ref icon);
            sc.Get(nameof(birthday), ref birthday);
            sc.Get(nameof(qq), ref qq);
            sc.Get(nameof(wechat), ref wechat);
            sc.Get(nameof(email), ref email);
            sc.Get(nameof(city), ref city);
            sc.Get(nameof(rank), ref rank);
            sc.Get(nameof(height), ref height);
            sc.Get(nameof(weight), ref weight);
            sc.Get(nameof(bust), ref bust);
            sc.Get(nameof(waist), ref waist);
            sc.Get(nameof(hip), ref hip);
            sc.Get(nameof(cup), ref cup);
            sc.Get(nameof(styles), ref styles);
            sc.Get(nameof(skills), ref skills);
            sc.Get(nameof(remark), ref remark);
            sc.Get(nameof(sites), ref sites, -1);
            sc.Get(nameof(friends), ref friends, -1);
            sc.Get(nameof(awards), ref awards, -1);
        }

        public void Save<R>(ISink<R> sk, int x) where R : ISink<R>
        {
            sk.Put(nameof(id), id);
            sk.Put(nameof(name), name);
            sk.Put(nameof(quote), quote);
            sk.Put(nameof(sex), sex);
            sk.Put(nameof(icon), icon);
            sk.Put(nameof(birthday), birthday);
            sk.Put(nameof(qq), qq);
            sk.Put(nameof(wechat), wechat);
            sk.Put(nameof(email), email);
            sk.Put(nameof(city), city);
            sk.Put(nameof(rank), rank);
            sk.Put(nameof(height), height);
            sk.Put(nameof(weight), weight);
            sk.Put(nameof(bust), bust);
            sk.Put(nameof(waist), waist);
            sk.Put(nameof(hip), hip);
            sk.Put(nameof(cup), cup);
            sk.Put(nameof(styles), styles);
            sk.Put(nameof(skills), skills);
            sk.Put(nameof(remark), remark);
            sk.Put(nameof(sites), sites, -1);
            sk.Put(nameof(friends), friends, -1);
            sk.Put(nameof(awards), awards, -1);
        }
    }

    public struct Item : IPersist
    {
        internal string uid;

        internal string url;

        internal string desc;

        public void Load(ISource sc, int x)
        {
            sc.Get(nameof(uid), ref uid);
            sc.Get(nameof(url), ref url);
            sc.Get(nameof(desc), ref desc);
        }

        public void Save<R>(ISink<R> sk, int x) where R : ISink<R>
        {
            sk.Put(nameof(uid), uid);
            sk.Put(nameof(url), url);
            sk.Put(nameof(desc), desc);
        }
    }
}