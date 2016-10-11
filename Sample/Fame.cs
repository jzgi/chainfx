using System;
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
        internal string careers; // comma-separated string
        internal string styles;
        internal string skills;
        internal short remark;
        internal Item[] sites;
        internal Item[] friends;
        internal Item[] awards;

        public void Load(ISource sc, ushort x = 0xffff)
        {
            sc.Got(nameof(id), ref id);
            sc.Got(nameof(name), ref name);
            sc.Got(nameof(quote), ref quote);
            sc.Got(nameof(sex), ref sex);
            sc.Got(nameof(icon), ref icon);
            sc.Got(nameof(birthday), ref birthday);
            sc.Got(nameof(qq), ref qq);
            sc.Got(nameof(wechat), ref wechat);
            sc.Got(nameof(email), ref email);
            sc.Got(nameof(city), ref city);
            sc.Got(nameof(rank), ref rank);
            sc.Got(nameof(height), ref height);
            sc.Got(nameof(weight), ref weight);
            sc.Got(nameof(bust), ref bust);
            sc.Got(nameof(waist), ref waist);
            sc.Got(nameof(hip), ref hip);
            sc.Got(nameof(cup), ref cup);
            sc.Got(nameof(careers), ref careers);
            sc.Got(nameof(styles), ref styles);
            sc.Got(nameof(skills), ref skills);
            sc.Got(nameof(remark), ref remark);
            sc.Got(nameof(sites), ref sites);
            sc.Got(nameof(friends), ref friends);
            sc.Got(nameof(awards), ref awards);
        }

        public void Save<R>(ISink<R> sk, ushort x = 0xffff) where R : ISink<R>
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
            sk.Put(nameof(careers), careers);
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

        internal string text;

        public void Load(ISource sc, ushort x = 0xffff)
        {
            sc.Got(nameof(uid), ref uid);
            sc.Got(nameof(url), ref url);
            sc.Got(nameof(text), ref text);
        }

        public void Save<R>(ISink<R> sk, ushort x = 0xffff) where R : ISink<R>
        {
            sk.Put(nameof(uid), uid);
            sk.Put(nameof(url), url);
            sk.Put(nameof(text), text);
        }
    }
}