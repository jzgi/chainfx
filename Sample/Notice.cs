using System;
using Greatbone.Core;
using static Greatbone.Core.DbSql;

namespace Greatbone.Sample
{
    public class Notice : IPersist
    {
        internal int id;

        internal string loc;

        internal string authorid;

        internal string author;

        internal DateTime date;

        internal DateTime duedate;

        internal string subject;

        internal string tel;

        internal string text;

        internal int reads;

        internal App[] apps;


        public void Load(ISource sc, uint x = 0)
        {
            sc.Got(nameof(id), ref id);
            sc.Got(nameof(loc), ref loc);
            sc.Got(nameof(authorid), ref authorid);
            sc.Got(nameof(author), ref author);
            sc.Got(nameof(date), ref date);
            sc.Got(nameof(duedate), ref duedate);
            sc.Got(nameof(subject), ref subject);
            sc.Got(nameof(tel), ref tel);
            sc.Got(nameof(text), ref text);
            sc.Got(nameof(reads), ref reads);
            sc.Got(nameof(apps), ref apps);
        }

        public void Save<R>(ISink<R> sk, uint x = 0) where R : ISink<R>
        {
            if (x.Neither(INS, UPD))
            {
                sk.Put(nameof(id), id);
            }
            sk.Put(nameof(loc), loc);
            sk.Put(nameof(authorid), authorid);
            sk.Put(nameof(author), author);
            sk.Put(nameof(date), date);
            sk.Put(nameof(duedate), duedate);
            sk.Put(nameof(subject), subject);
            sk.Put(nameof(tel), tel);
            sk.Put(nameof(text), text);
            sk.Put(nameof(reads), reads);
            sk.Put(nameof(apps), apps);
        }
    }

    internal struct App : IPersist
    {
        internal string userid;

        internal string user;

        public void Load(ISource sc, uint x = 0)
        {
            sc.Got(nameof(userid), ref userid);
            sc.Got(nameof(user), ref user);
        }

        public void Save<R>(ISink<R> sk, uint x = 0) where R : ISink<R>
        {
            sk.Put(nameof(userid), userid);
            sk.Put(nameof(user), user);
        }
    }
}