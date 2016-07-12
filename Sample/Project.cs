using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Project : IData
    {
        internal int id;

        internal DateTime opened;

        internal string title;

        internal string description;

        internal int offererId;

        internal int acceptorId;

        internal int term;



        public void From(IDataInput i)
        {
        }

        public void To(IDataOutput o)
        {
        }
    }
}