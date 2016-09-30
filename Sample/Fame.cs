using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{

    [Flags]
    public enum FameStates
    {

    }

    public struct Fame : IData
    {
        public FameStates states;

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

        public void In(IDataIn i)
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

        public void Out<R>(IDataOut<R> o) where R : IDataOut<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(name), name);
            o.Put(nameof(quote), quote);
            o.Put(nameof(sex), sex);
            o.Put(nameof(icon), icon);
            o.Put(nameof(birthday), birthday);
            o.Put(nameof(qq), qq);
            o.Put(nameof(wechat), wechat);
            o.Put(nameof(email), email);
            o.Put(nameof(city), city);
            o.Put(nameof(rank), rank);
            o.Put(nameof(height), height);
            o.Put(nameof(weight), weight);
            o.Put(nameof(bust), bust);
            o.Put(nameof(waist), waist);
            o.Put(nameof(hip), hip);
            o.Put(nameof(cup), cup);
            o.Put(nameof(styles), styles);
            o.Put(nameof(skills), skills);
            o.Put(nameof(remark), remark);
            o.Put(nameof(sites), sites);
            o.Put(nameof(friends), friends);
            o.Put(nameof(awards), awards);
        }
    }

    public struct Item
    {
        internal char[] uid;

        internal string url;

        internal string desc;

        public void In(IDataIn i)
        {
            i.Get(nameof(uid), ref uid);
            i.Get(nameof(url), ref url);
            i.Get(nameof(desc), ref desc);
        }

        public void Out<R>(IDataOut<R> o) where R : IDataOut<R>
        {
            o.Put(nameof(uid), uid);
            o.Put(nameof(url), url);
            o.Put(nameof(desc), desc);
        }
    }
}