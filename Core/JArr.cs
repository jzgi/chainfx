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

        internal void Add(JMember e)
        {
            int len = elements.Length;
            if (count >= len)
            {
                JMember[] all = new JMember[len * 4];
                Array.Copy(elements, all, len);
                elements = all;
            }
            elements[count++] = e;
        }

        internal void Save<R>(ISink<R> snk) where R : ISink<R>
        {
            for (int i = 0; i < count; i++)
            {
                JMember elem = elements[i];
                sbyte vt = elem.vt;
                if (vt == VT.Array)
                {
                    snk.Put((JArr)elem);
                }
                else if (vt == VT.Object)
                {
                    snk.Put((JObj)elem);
                }
                else if (vt == VT.String)
                {
                    snk.Put((string)elem);
                }
                else if (vt == VT.Number)
                {
                    snk.Put((Number)elem);
                }
                else if (vt == VT.True)
                {
                    snk.Put(true);
                }
                else if (vt == VT.False)
                {
                    snk.Put(false);
                }
                else if (vt == VT.Null)
                {
                    snk.PutNull();
                }
            }
        }


        public P[] ToArr<P>(byte x = 0xff) where P : IPersist, new()
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