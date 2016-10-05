using System;
using System.Collections.Generic;
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

        internal short subtype;

        internal string subject;

        internal string remark;

        internal List<Ask> asks;


        public void Load(ISource sc)
        {
            sc.Got(nameof(id), ref id);
            sc.Got(nameof(loc), ref loc);
            sc.Got(nameof(authorid), ref authorid);
            sc.Got(nameof(author), ref author);
            sc.Got(nameof(date), ref date);
            sc.Got(nameof(duedate), ref duedate);
            sc.Got(nameof(subtype), ref subtype);
            sc.Got(nameof(subject), ref subject);
            sc.Got(nameof(remark), ref remark);
            sc.Got(nameof(asks), ref asks);
        }

        public void Save<R>(ISink<R> sk) where R : ISink<R>
        {
            sk.Put(nameof(id), id);
            sk.Put(nameof(loc), loc);
            sk.Put(nameof(authorid), authorid);
            sk.Put(nameof(author), author);
            sk.Put(nameof(date), date);
            sk.Put(nameof(duedate), duedate);
            sk.Put(nameof(subtype), subtype);
            sk.Put(nameof(subject), subject);
            sk.Put(nameof(remark), remark);
            sk.Put(nameof(asks), asks);
        }
    }

    internal struct Ask : IPersist
    {
        internal string id;

        public void Load(ISource sc)
        {
            throw new NotImplementedException();
        }

        public void Save<R>(ISink<R> sk) where R : ISink<R>
        {
            throw new NotImplementedException();
        }
    }
}