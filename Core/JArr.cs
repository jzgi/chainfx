using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// A JSON array model.
    ///
    public class JArr : IModel, ISourceSet
    {
        // array elements
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

        internal void Add(JMem mem)
        {
            int len = elements.Length;
            if (count >= len)
            {
                JMem[] alloc = new JMem[len * 4];
                Array.Copy(elements, 0, alloc, 0, len);
                elements = alloc;
            }
            elements[count++] = mem;
        }

        public bool Single => false;

        public void Dump<R>(ISink<R> snk) where R : ISink<R>
        {
            for (int i = 0; i < count; i++)
            {
                JMem mem = elements[i];
                JType t = mem.type;
                if (t == JType.Array)
                {
                    snk.Put(null, (JArr)mem);
                }
                else if (t == JType.Object)
                {
                    snk.Put(null, (JObj)mem);
                }
                else if (t == JType.String)
                {
                    snk.Put(null, (string)mem);
                }
                else if (t == JType.Number)
                {
                    snk.Put(null, (JNumber)mem);
                }
                else if (t == JType.True)
                {
                    snk.Put(null, true);
                }
                else if (t == JType.False)
                {
                    snk.Put(null, false);
                }
                else if (t == JType.Null)
                {
                    snk.PutNull(null);
                }
            }
        }

        public C Dump<C>() where C : IContent, ISink<C>, new()
        {
            C cont = new C();
            Dump(cont);
            return cont;
        }


        public bool Get(string name, ref bool v)
        {
            JObj jobj = elements[current];
            return jobj != null && jobj.Get(name, ref v);
        }

        public bool Get(string name, ref short v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref int v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref long v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref double v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref decimal v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref DateTime v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref NpgsqlPoint v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref char[] v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref string v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref byte[] v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get<D>(string name, ref D v, byte flags = 0) where D : IData, new()
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref JObj v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref JArr v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref short[] v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref int[] v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref long[] v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref string[] v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get<D>(string name, ref D[] v, byte flags = 0) where D : IData, new()
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get<D>(string name, ref List<D> v, byte flags = 0) where D : IData, new()
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Next()
        {
            return ++current < count;
        }

        public override string ToString()
        {
            JsonContent cont = new JsonContent(false, true);
            cont.Put(null, this);
            string str = cont.ToString();
            BufferUtility.Return(cont);
            return str;
        }

        public D ToObject<D>(byte flags = 0) where D : IData, new()
        {
            D obj = new D();
            obj.Load(this, flags);
            return obj;
        }

        public D[] ToArray<D>(byte flags = 0) where D : IData, new()
        {
            D[] arr = new D[count];
            for (int i = 0; i < arr.Length; i++)
            {
                D obj = new D();
                obj.Load((JObj)elements[i], flags);
                arr[i] = obj;
            }
            return arr;
        }

        public List<D> ToList<D>(byte flags = 0) where D : IData, new()
        {
            List<D> lst = new List<D>(count + 8);
            for (int i = 0; i < count; i++)
            {
                D obj = new D();
                obj.Load((JObj)elements[i], flags);
                lst.Add(obj);
            }
            return lst;
        }
    }
}