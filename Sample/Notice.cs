using System;
using Greatbone.Core;

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


        public void Load(ISource s, uint x = 0)
        {
            s.Got(nameof(id), ref id);
            s.Got(nameof(loc), ref loc);
            s.Got(nameof(authorid), ref authorid);
            s.Got(nameof(author), ref author);
            s.Got(nameof(date), ref date);
            s.Got(nameof(duedate), ref duedate);
            s.Got(nameof(subject), ref subject);
            s.Got(nameof(tel), ref tel);
            s.Got(nameof(text), ref text);
            s.Got(nameof(reads), ref reads);
            s.Got(nameof(apps), ref apps);
        }

        public void Save<R>(ISink<R> s, uint x = 0) where R : ISink<R>
        {
            if (x.DefaultOn())
            {
                s.Put(nameof(id), id);
            }
            s.Put(nameof(loc), loc);
            s.Put(nameof(authorid), authorid);
            s.Put(nameof(author), author);
            s.Put(nameof(date), date);
            s.Put(nameof(duedate), duedate);
            s.Put(nameof(subject), subject);
            s.Put(nameof(tel), tel);
            s.Put(nameof(text), text);
            s.Put(nameof(reads), reads);
            s.Put(nameof(apps), apps);
        }
    }

    internal struct App : IPersist
    {
        internal string userid;
        internal string user;

        public void Load(ISource s, uint x = 0)
        {
            s.Got(nameof(userid), ref userid);
            s.Got(nameof(user), ref user);
        }

        public void Save<R>(ISink<R> s, uint x = 0) where R : ISink<R>
        {
            s.Put(nameof(userid), userid);
            s.Put(nameof(user), user);
        }
    }
}