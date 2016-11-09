using System;
using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Greatbone.Sample
{

    public class Post : IBean
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
        internal string mset; // available m fields, such as "2384"
        internal byte[] m0, m1, m2, m3, m4, m5, m6, m7, m8;

        public void Load(ISource s, byte z = 0)
        {
            if (z.Ya(AUTO)) s.Get(nameof(id), ref id);
            s.Get(nameof(time), ref time);
            s.Get(nameof(authorid), ref authorid);
            s.Get(nameof(author), ref author);
            s.Get(nameof(commentable), ref commentable);
            s.Get(nameof(comments), ref comments);
            s.Get(nameof(likes), ref likes);
            s.Get(nameof(shared), ref shared);
            s.Get(nameof(text), ref text);
            s.Get(nameof(mset), ref mset);

            if (z.Ya(BIN))
            {
                s.Get(nameof(m0), ref m0);
                s.Get(nameof(m1), ref m1);
                s.Get(nameof(m2), ref m2);
                s.Get(nameof(m3), ref m3);
                s.Get(nameof(m4), ref m4);
                s.Get(nameof(m5), ref m5);
                s.Get(nameof(m6), ref m6);
                s.Get(nameof(m7), ref m7);
                s.Get(nameof(m8), ref m8);
            }
        }

        public void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>
        {
            if (z.Ya(AUTO)) s.Put(nameof(id), id);
            s.Put(nameof(time), time);
            s.Put(nameof(authorid), authorid);
            s.Put(nameof(author), author);
            s.Put(nameof(commentable), commentable);
            s.Put(nameof(comments), comments);
            s.Put(nameof(likes), likes);
            s.Put(nameof(shared), shared);
            s.Put(nameof(text), text);
            s.Put(nameof(mset), mset);

            if (z.Ya(BIN))
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