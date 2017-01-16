using System;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// A JSON array model.
    ///
    public class JArr : IContentModel, ISourceSet
    {
        JMem[] elements;

        int count;

        int current;

        internal JArr(int capacity = 16)
        {
            elements = new JMem[capacity];
            count = 0;
            current = 0;
        }

        public ISource SourceItem(int index)
        {
            return (JObj) elements[index];
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

        public void Dump<R>(ISink<R> snk) where R : ISink<R>
        {
            for (int i = 0; i < count; i++)
            {
                JMem elem = elements[i];
                JType typ = elem.type;
                if (typ == JType.Array)
                {
                    snk.Put(null, (JArr) elem);
                }
                else if (typ == JType.Object)
                {
                    snk.Put(null, (JObj) elem);
                }
                else if (typ == JType.String)
                {
                    snk.Put(null, (string) elem);
                }
                else if (typ == JType.Number)
                {
                    snk.Put(null, (JNumber) elem);
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

        public bool Get(string name, ref bool v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref int v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref long v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref double v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref decimal v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref DateTime v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref NpgsqlPoint v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref char[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref string v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref byte[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D v, byte flags = 0) where D : IDat, new()
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref JObj v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref JArr v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref int[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref long[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref string[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D[] v, byte flags = 0) where D : IDat, new()
        {
            throw new NotImplementedException();
        }

        public bool Next()
        {
            throw new NotImplementedException();
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

        public D ToDat<D>(byte flags = 0) where D : IDat, new()
        {
            D dat = new D();
            dat.Load(this, flags);
            return dat;
        }

        public D[] ToDats<D>(byte flags = 0) where D : IDat, new()
        {
            D[] arr = new D[count];
            for (int i = 0; i < arr.Length; i++)
            {
                D dat = new D();
                dat.Load((JObj) this[i], flags);
                arr[i] = dat;
            }
            return arr;
        }
    }
}