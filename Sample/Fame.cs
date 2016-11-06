using System;
using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Greatbone.Sample
{

    public struct Fame : IPersist
    {
        public static Fame Empty = new Fame();

        internal string id;
        internal string name;
        internal string quote;
        internal string sex;
        internal byte[] icon;
        internal DateTime birthday;
        internal string qq;
        internal string wechat;
        internal string email;
        internal string city;
        internal short rating;
        internal short height;
        internal short weight;
        internal short bust;
        internal short waist;
        internal short hip;
        internal short cup;
        internal string[] styles;
        internal string[] skills;
        internal string remark;
        internal Ref[] sites;
        internal Ref[] friends;

        public void Load(ISource s, byte z = 0)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(name), ref name);
            s.Get(nameof(quote), ref quote);
            s.Get(nameof(sex), ref sex);
            if (z.Ya(BIN)) s.Get(nameof(icon), ref icon);
            s.Get(nameof(birthday), ref birthday);
            s.Get(nameof(qq), ref qq);
            s.Get(nameof(wechat), ref wechat);
            s.Get(nameof(email), ref email);
            s.Get(nameof(city), ref city);
            s.Get(nameof(rating), ref rating);
            s.Get(nameof(height), ref height);
            s.Get(nameof(weight), ref weight);
            s.Get(nameof(bust), ref bust);
            s.Get(nameof(waist), ref waist);
            s.Get(nameof(hip), ref hip);
            s.Get(nameof(cup), ref cup);
            if (z.Ya(DEEP)) s.Get(nameof(styles), ref styles);
            if (z.Ya(DEEP)) s.Get(nameof(skills), ref skills);
            s.Get(nameof(remark), ref remark);
            if (z.Ya(DEEP)) s.Get(nameof(sites), ref sites, z);
            if (z.Ya(DEEP)) s.Get(nameof(friends), ref friends, z);
        }

        public void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            s.Put(nameof(quote), quote);
            s.Put(nameof(sex), sex);
            if (z.Ya(BIN)) s.Put(nameof(icon), icon);
            s.Put(nameof(birthday), birthday);
            s.Put(nameof(qq), qq);
            s.Put(nameof(wechat), wechat);
            s.Put(nameof(email), email);
            s.Put(nameof(city), city);
            s.Put(nameof(rating), rating);
            s.Put(nameof(height), height);
            s.Put(nameof(weight), weight);
            s.Put(nameof(bust), bust);
            s.Put(nameof(waist), waist);
            s.Put(nameof(hip), hip);
            s.Put(nameof(cup), cup);
            if (z.Ya(DEEP)) s.Put(nameof(styles), styles);
            if (z.Ya(DEEP)) s.Put(nameof(skills), skills);
            s.Put(nameof(remark), remark);
            if (z.Ya(DEEP)) s.Put(nameof(sites), sites, z);
            if (z.Ya(DEEP)) s.Put(nameof(friends), friends, z);
        }

    }

    public struct Ref : IPersist
    {
        internal string name;

        // an id or url
        internal string @ref;

        internal string hint;

        public void Load(ISource s, byte z = 0)
        {
            s.Get(nameof(name), ref name);
            s.Get(nameof(@ref), ref @ref);
            s.Get(nameof(hint), ref hint);
        }

        public void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>
        {
            s.Put(nameof(name), name);
            s.Put(nameof(@ref), @ref);
            s.Put(nameof(hint), hint);
        }

    }

}