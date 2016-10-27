using System;
using Greatbone.Core;
using static Greatbone.Core.XUtility;

namespace Greatbone.Sample
{
    public class Notice : IPersist
    {
        public static Notice Empty = new Notice();

        internal int id;
        internal string loc;
        internal string authorid;
        internal string author;
        internal DateTime date;
        internal DateTime duedate;
        internal string subject;
        internal string tel;
        internal string text;
        internal int read;
        internal int shared;
        internal App[] apps;
        internal bool commentable;
        internal Comment[] comments;


        public void Load(ISource s, byte x = 0xff)
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
            s.Got(nameof(read), ref read);
            s.Got(nameof(shared), ref shared);

            if (x.On(EXTRA))
                s.Got(nameof(apps), ref apps);

            s.Got(nameof(commentable), ref commentable);

            if (x.On(EXTRA))
                s.Got(nameof(comments), ref comments);
        }

        public void Save<R>(ISink<R> s, byte x = 0xff) where R : ISink<R>
        {
            if (x.On(AUTO))
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
            s.Put(nameof(read), read);
            s.Put(nameof(shared), shared);

            if (x.On(EXTRA))
                s.Put(nameof(apps), apps, x);

            s.Put(nameof(commentable), commentable);

            if (x.On(EXTRA))
                s.Put(nameof(comments), comments, x);
        }
    }

    internal struct App : IPersist
    {
        internal string userid;
        internal string user;

        public void Load(ISource s, byte x = 0xff)
        {
            s.Got(nameof(userid), ref userid);
            s.Got(nameof(user), ref user);
        }

        public void Save<R>(ISink<R> s, byte x = 0xff) where R : ISink<R>
        {
            s.Put(nameof(userid), userid);
            s.Put(nameof(user), user);
        }
    }
}