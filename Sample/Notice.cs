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
            sc.Got(nameof(id), out id);
            sc.Got(nameof(loc), out loc);
            sc.Got(nameof(authorid), out authorid);
            sc.Got(nameof(author), out author);
            sc.Got(nameof(date), out date);
            sc.Got(nameof(duedate), out duedate);
            sc.Got(nameof(subtype), out subtype);
            sc.Got(nameof(subject), out subject);
            sc.Got(nameof(remark), out remark);
            sc.Got(nameof(asks), out asks);
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