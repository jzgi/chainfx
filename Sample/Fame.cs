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

        public void Load(ISource sc)
        {
            sc.Got(nameof(id), out id);
            sc.Got(nameof(name), out name);
            sc.Got(nameof(quote), out quote);
            sc.Got(nameof(sex), out sex);
            sc.Got(nameof(icon), out icon);
            sc.Got(nameof(birthday), out birthday);
            sc.Got(nameof(qq), out qq);
            sc.Got(nameof(wechat), out wechat);
            sc.Got(nameof(email), out email);
            sc.Got(nameof(city), out city);
            sc.Got(nameof(rank), out rank);
            sc.Got(nameof(height), out height);
            sc.Got(nameof(weight), out weight);
            sc.Got(nameof(bust), out bust);
            sc.Got(nameof(waist), out waist);
            sc.Got(nameof(hip), out hip);
            sc.Got(nameof(cup), out cup);
            sc.Got(nameof(styles), out styles);
            sc.Got(nameof(skills), out skills);
            sc.Got(nameof(remark), out remark);
            sc.Got(nameof(sites), out sites);
            sc.Got(nameof(friends), out friends);
            sc.Got(nameof(awards), out awards);
        }

        public void Save<R>(ISink<R> sk) where R : ISink<R>
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
            sk.Put(nameof(sites), sites);
            sk.Put(nameof(friends), friends);
            sk.Put(nameof(awards), awards);
        }
    }

    public struct Item : IPersist
    {
        internal string uid;

        internal string url;

        internal string desc;

        public void Load(ISource sc)
        {
            sc.Got(nameof(uid), out uid);
            sc.Got(nameof(url), out url);
            sc.Got(nameof(desc), out desc);
        }

        public void Save<R>(ISink<R> sk) where R : ISink<R>
        {
            sk.Put(nameof(uid), uid);
            sk.Put(nameof(url), url);
            sk.Put(nameof(desc), desc);
        }
    }
}