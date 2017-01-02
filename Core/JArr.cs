using System;

namespace Greatbone.Core
{
    ///
    /// A JSON array model.
    ///
    public class JArr
    {
        const int InitialCapacity = 16;

        JMem[] elements;

        int count;

        internal JArr(int capacity = InitialCapacity)
        {
            elements = new JMem[capacity];
            count = 0;
        }

        public JMem this[int index] => elements[index];

        public int Count => count;

        internal void Add(JMem elem)
        {
            int len = elements.Length;
            if (count >= len)
            {
                JMem[] alloc = new JMem[len * 4];
                Array.Copy(elements, 0, alloc, 0, len);
                elements = alloc;
            }
            elements[count++] = elem;
        }

        internal void Dump<R>(ISink<R> snk) where R : ISink<R>
        {
            for (int i = 0; i < count; i++)
            {
                JMem elem = elements[i];
                JType typ = elem.type;
                if (typ == JType.Array)
                {
                    snk.Put(null, (JArr)elem);
                }
                else if (typ == JType.Object)
                {
                    snk.Put(null, (JObj)elem);
                }
                else if (typ == JType.String)
                {
                    snk.Put(null, (string)elem);
                }
                else if (typ == JType.Number)
                {
                    snk.Put(null, (JNum)elem);
                }
                else if (typ == JType.True)
                {
                    snk.Put(null, true);
                }
                else if (typ == JType.False)
                {
                    snk.Put(null, false);
                }
                else if (typ == JType.Null)
                {
                    snk.PutNull(null);
                }
            }
        }

        public D[] ToDataArr<D>(byte bits = 0) where D : IData, new()
        {
            D[] arr = new D[count];
            for (int i = 0; i < arr.Length; i++)
            {
                D obj = new D();
                obj.Load((JObj)this[i], bits);
                arr[i] = obj;
            }
            return arr;
        }

        public override string ToString()
        {
            JsonContent cont = new JsonContent(false, true);
            cont.Add('[');
            Dump(cont);
            cont.Add(']');
            string str = cont.ToString();
            BufferUtility.Return(cont);
            return str;
        }
    }
}