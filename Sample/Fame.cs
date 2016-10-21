using System;
using Greatbone.Core;

namespace Greatbone.Sample
{

    public struct Fame : IPersist
    {
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

        public void Load(ISource s, uint x = 0)
        {
            s.Got(nameof(id), ref id);
            s.Got(nameof(name), ref name);
            s.Got(nameof(quote), ref quote);
            s.Got(nameof(sex), ref sex);
            if (x.BinaryOn())
            {
                s.Got(nameof(icon), ref icon);
            }
            s.Got(nameof(birthday), ref birthday);
            s.Got(nameof(qq), ref qq);
            s.Got(nameof(wechat), ref wechat);
            s.Got(nameof(email), ref email);
            s.Got(nameof(city), ref city);
            s.Got(nameof(rating), ref rating);
            s.Got(nameof(height), ref height);
            s.Got(nameof(weight), ref weight);
            s.Got(nameof(bust), ref bust);
            s.Got(nameof(waist), ref waist);
            s.Got(nameof(hip), ref hip);
            s.Got(nameof(cup), ref cup);
            s.Got(nameof(styles), ref styles);
            s.Got(nameof(skills), ref skills);
            s.Got(nameof(remark), ref remark);
            s.Got(nameof(sites), ref sites);
            s.Got(nameof(friends), ref friends);
        }

        public void Save<R>(ISink<R> s, uint x = 0) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            s.Put(nameof(quote), quote);
            s.Put(nameof(sex), sex);
            if (x.BinaryOn())
            {
                s.Put(nameof(icon), icon);
            }
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
            s.Put(nameof(styles), styles);
            s.Put(nameof(skills), skills);
            s.Put(nameof(remark), remark);
            s.Put(nameof(sites), sites);
            s.Put(nameof(friends), friends);
        }
    }

    public struct Ref : IPersist
    {
        internal string name;

        // an id or url
        internal string @ref;

        internal string hint;

        public void Load(ISource s, uint x = 0)
        {
            s.Got(nameof(name), ref name);
            s.Got(nameof(@ref), ref @ref);
            s.Got(nameof(hint), ref hint);
        }

        public void Save<R>(ISink<R> s, uint x = 0) where R : ISink<R>
        {
            s.Put(nameof(name), name);
            s.Put(nameof(@ref), @ref);
            s.Put(nameof(hint), hint);
        }
    }

}