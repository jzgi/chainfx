using System;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// A JSON array model.
    /// </summary>
    ///
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

        internal void Add(JMember mem)
        {
            int len = elements.Length;
            if (count >= len)
            {
                JMember[] alloc = new JMember[len * 4];
                Array.Copy(elements, 0, alloc, 0, len);
                elements = alloc;
            }
            elements[count++] = mem;
        }

        internal void Dump<R>(ISink<R> snk) where R : ISink<R>
        {
            for (int i = 0; i < count; i++)
            {
                JMember mbr = elements[i];
                JType typ = mbr.type;
                if (typ == JType.Array)
                {
                    snk.Put((JArr)mbr);
                }
                else if (typ == JType.Object)
                {
                    snk.Put((JObj)mbr);
                }
                else if (typ == JType.String)
                {
                    snk.Put((string)mbr);
                }
                else if (typ == JType.Number)
                {
                    snk.Put((Number)mbr);
                }
                else if (typ == JType.True)
                {
                    snk.Put(true);
                }
                else if (typ == JType.False)
                {
                    snk.Put(false);
                }
                else if (typ == JType.Null)
                {
                    snk.PutNull();
                }
            }
        }

        public P[] ToArr<P>(byte x = 0) where P : IPersist, new()
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