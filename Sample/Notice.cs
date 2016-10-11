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

        internal short subtype;

        internal string subject;

        internal string contact;

        internal string remark;

        internal Apply[] applies;


        public void Load(ISource sc, ushort x = 0xffff)
        {
            sc.Got(nameof(id), ref id);
            sc.Got(nameof(loc), ref loc);
            sc.Got(nameof(authorid), ref authorid);
            sc.Got(nameof(author), ref author);
            sc.Got(nameof(date), ref date);
            sc.Got(nameof(duedate), ref duedate);
            sc.Got(nameof(subtype), ref subtype);
            sc.Got(nameof(subject), ref subject);
            sc.Got(nameof(contact), ref contact);
            sc.Got(nameof(remark), ref remark);
            sc.Got(nameof(applies), ref applies);
        }

        public void Save<R>(ISink<R> sk, ushort x = 0xffff) where R : ISink<R>
        {
            sk.Put(nameof(id), id);
            sk.Put(nameof(loc), loc);
            sk.Put(nameof(authorid), authorid);
            sk.Put(nameof(author), author);
            sk.Put(nameof(date), date);
            sk.Put(nameof(duedate), duedate);
            sk.Put(nameof(subtype), subtype);
            sk.Put(nameof(subject), subject);
            sk.Put(nameof(contact), contact);
            sk.Put(nameof(remark), remark);
            sk.Put(nameof(applies), applies);
        }
    }

    internal struct Apply : IPersist
    {
        internal string userid;

        internal string user;

        public void Load(ISource sc, ushort x = 0xffff)
        {
            sc.Got(nameof(userid), ref userid);
            sc.Got(nameof(user), ref user);
        }

        public void Save<R>(ISink<R> sk, ushort x = 0xffff) where R : ISink<R>
        {
            sk.Put(nameof(userid), userid);
            sk.Put(nameof(user), user);
        }
    }
}