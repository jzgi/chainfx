using System;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// A JSON array model.
    /// </summary>
    ///
    public class Arr
    {
        const int InitialCapacity = 16;

        Member[] elements;

        int count;

        internal Arr(int capacity = InitialCapacity)
        {
            elements = new Member[capacity];
            count = 0;
        }

        public Member this[int index]
        {
            get { return elements[index]; }
        }

        public int Count => count;

        internal void Add(Member mem)
        {
            int len = elements.Length;
            if (count >= len)
            {
                Member[] alloc = new Member[len * 4];
                Array.Copy(elements, 0, alloc, 0, len);
                elements = alloc;
            }
            elements[count++] = mem;
        }

        internal void Dump<R>(ISink<R> snk) where R : ISink<R>
        {
            for (int i = 0; i < count; i++)
            {
                Member mbr = elements[i];
                MemberType typ = mbr.type;
                if (typ == MemberType.Array)
                {
                    snk.Put((Arr)mbr);
                }
                else if (typ == MemberType.Object)
                {
                    snk.Put((Obj)mbr);
                }
                else if (typ == MemberType.String)
                {
                    snk.Put((string)mbr);
                }
                else if (typ == MemberType.Number)
                {
                    snk.Put((Number)mbr);
                }
                else if (typ == MemberType.True)
                {
                    snk.Put(true);
                }
                else if (typ == MemberType.False)
                {
                    snk.Put(false);
                }
                else if (typ == MemberType.Null)
                {
                    snk.PutNull();
                }
            }
        }

        public B[] ToBeans<B>(byte z = 0) where B : IBean, new()
        {
            B[] beans = new B[count];
            for (int i = 0; i < beans.Length; i++)
            {
                B bean = new B();
                bean.Load((Obj)this[i], z);
                beans[i] = bean;
            }
            return beans;
        }

    }

}