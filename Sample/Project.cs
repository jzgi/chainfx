using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Project : IPersistable
    {
        internal int id;

        internal DateTime opened;

        internal string title;

        internal string description;

        internal int offererId;

        internal int acceptorId;

        internal int term;



        public void From(ISource s)
        {
        }

        public void To(ITarget t)
        {
        }
    }
}