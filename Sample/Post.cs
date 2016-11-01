using System;
using Greatbone.Core;
using static Greatbone.Core.XUtility;

namespace Greatbone.Sample
{

    public class Post : IPersist
    {
        public static Post Empty = new Post();

        internal int id;
        internal DateTime time;
        internal string authorid;
        internal string author;
        internal bool commentable;
        internal Comment[] comments;
        internal string[] likes;
        internal int shared;
        internal string text;
        // indices of available m fields, such as "2384"
        internal string mset;
        internal byte[] m0, m1, m2, m3, m4, m5, m6, m7, m8;

        public void Load(ISource s, byte x = 0xff)
        {
            s.Got(nameof(id), ref id);
            s.Got(nameof(time), ref time);
            s.Got(nameof(authorid), ref authorid);
            s.Got(nameof(author), ref author);
            s.Got(nameof(commentable), ref commentable);
            s.Got(nameof(comments), ref comments);
            s.Got(nameof(likes), ref likes);
            s.Got(nameof(shared), ref shared);
            s.Got(nameof(text), ref text);
            s.Got(nameof(mset), ref mset);

            if (x.On(BIN))
            {
                s.Got(nameof(m0), ref m0);
                s.Got(nameof(m1), ref m1);
                s.Got(nameof(m2), ref m2);
                s.Got(nameof(m3), ref m3);
                s.Got(nameof(m4), ref m4);
                s.Got(nameof(m5), ref m5);
                s.Got(nameof(m6), ref m6);
                s.Got(nameof(m7), ref m7);
                s.Got(nameof(m8), ref m8);
            }
        }

        public void Dump<R>(ISink<R> s, byte x = 0xff) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(time), time);
            s.Put(nameof(authorid), authorid);
            s.Put(nameof(author), author);
            s.Put(nameof(commentable), commentable);
            s.Put(nameof(comments), comments);
            s.Put(nameof(likes), likes);
            s.Put(nameof(shared), shared);
            s.Put(nameof(text), text);
            s.Put(nameof(mset), mset);

            if (x.On(BIN))
            {
                s.Put(nameof(m0), m0);
                s.Put(nameof(m1), m1);
                s.Put(nameof(m2), m2);
                s.Put(nameof(m3), m3);
                s.Put(nameof(m4), m4);
                s.Put(nameof(m5), m5);
                s.Put(nameof(m6), m6);
                s.Put(nameof(m7), m7);
                s.Put(nameof(m8), m8);
            }
        }

    }

}