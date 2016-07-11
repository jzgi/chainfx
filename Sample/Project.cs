using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Project : IDump
    {
        internal int id;

        internal DateTime opened;

        internal string title;

        internal string description;

        internal int offererId;

        internal int acceptorId;

        internal int term;



        public void From(IInput i)
        {
        }

        public void To(IOutput o)
        {
        }
    }
}