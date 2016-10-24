using System;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// The descriptor of an interface that defines a group of action methods.
    /// </summary>
    ///
    public class WebInterface
    {
        const int InitialCapacity = 8;

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
            // ensure capacity
            int olen = actions.Length;
            if (count >= olen)
            {
                WebAction[] alloc = new WebAction[olen * 2];
                Array.Copy(actions, 0, alloc, 0, olen);
                actions = alloc;
            }
            // append
            actions[count++] = a;
        }

        public int Count => count;

        public WebAction this[int index] => actions[index];
    }
}