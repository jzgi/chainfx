using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Notice : ISerial
    {
        internal int id;

        internal string loc;

        internal char[] authorid;

        internal string author;

        internal DateTime date;

        internal DateTime duedate;

        internal short subtype;

        internal string subject;

        internal string remark;

        internal int reads;

        internal List<Join> joins;


        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(id), ref id);
            r.Read(nameof(loc), ref loc);
            r.Read(nameof(authorid), ref authorid);
            r.Read(nameof(subject), ref subject);
        }

        public void WriteTo(ISerialWriter w)
        {
            w.Write(nameof(id), id);
            w.Write(nameof(subject), subject);
        }
    }

    internal struct Join : ISerial
    {
        internal char[] id;

        public void ReadFrom(ISerialReader r)
        {
            throw new NotImplementedException();
        }

        public void WriteTo(ISerialWriter w)
        {
            throw new NotImplementedException();
        }
    }
}