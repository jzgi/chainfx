using System;

namespace Greatbone.Core
{
    /// <summary>
    /// An array JSON data model.
    /// </summary>
    public class JArr
    {
        const int InitialCapacity = 16;

        JMember[] elements;

        int count;

        internal JArr(int capacity = InitialCapacity)
        {
            elements = new JMember[capacity];
            count = 0;
        }

        public JMember this[int index]
        {
            get { return elements[index]; }
        }

        public int Count => count;

        internal void Add(JMember elem)
        {
            int len = elements.Length;
            if (count >= len)
            {
                JMember[] @new = new JMember[len * 4];
                Array.Copy(elements, @new, len);
                elements = @new;
            }
            elements[count++] = elem;
        }

        internal void Save<R>(ISink<R> sk) where R : ISink<R>
        {
            for (int i = 0; i < count; i++)
            {
                JMember elem = elements[i];
                VT vt = elem.vt;
                if (vt == VT.Array)
                {
                    sk.Put((JArr)elem);
                }
                else if (vt == VT.Object)
                {
                    sk.Put((JObj)elem);
                }
                else if (vt == VT.String)
                {
                    sk.Put((string)elem);
                }
                else if (vt == VT.Number)
                {
                    sk.Put((Number)elem);
                }
                else if (vt == VT.True)
                {
                    sk.Put(true);
                }
                else if (vt == VT.False)
                {
                    sk.Put(false);
                }
                else if (vt == VT.Null)
                {
                    sk.PutNull();
                }
            }
        }


        public P[] ToArr<P>(uint x = 0) where P : IPersist, new()
        {
            P[] arr = new P[count];
            for (int i = 0; i < arr.Length; i++)
            {
                P obj = new P();
                obj.Load((JObj)this[i], x);
                arr[i] = obj;
            }
            return arr;
        }

    }
}