using System;

namespace Greatbone.Core
{
    public class WebInterface
    {
        const int InitialCapacity = 16;

        readonly Type type;

        WebAction[] actions;

        int count;

        internal WebInterface(Type type)
        {
            this.type = type;
            actions = new WebAction[InitialCapacity];
        }

        public Type Type => type;

        internal void Add(WebAction a)
        {
            actions[count++] = a;
        }

        public int Count => count;

        public WebAction this[int index] => actions[index];
    }
}