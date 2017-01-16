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
            current = -1;
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
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref short v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref int v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref long v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref double v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref decimal v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref DateTime v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref NpgsqlPoint v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref char[] v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref string v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref byte[] v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get<D>(string name, ref D v, byte flags = 0) where D : IDat, new()
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref JObj v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref JArr v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref short[] v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref int[] v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref long[] v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref string[] v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get<D>(string name, ref D[] v, byte flags = 0) where D : IDat, new()
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Next()
        {
            return ++current < count;
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
            D[] dats = new D[count];
            for (int i = 0; i < dats.Length; i++)
            {
                D dat = new D();
                dat.Load((JObj) this[i], flags);
                dats[i] = dat;
            }
            return dats;
        }
    }
}