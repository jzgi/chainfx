using System;
using System.Threading;

namespace Greatbone.Core
{
    /// 
    /// A event queue pertaining to a certain event client.
    /// 
    public class EventQueue : IRollable
    {
        readonly string name;

        readonly Event[] elements;

        int head;

        int tail;

        int count;

        internal EventQueue(string name, int capacity)
        {
            this.name = name;
            elements = new Event[capacity];
        }

        public string Name => name;

        public void Remove(string target)
        {
            return;
        }

        public void Clear()
        {
            return;
        }

        internal void Clean()
        {
        }
    }
}