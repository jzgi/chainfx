using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// A JSON array model.
    ///
    public class JArr : IDataInput
    {
        // array elements
        JMbr[] elements;

        int count;

        int current;

        internal JArr(int capacity = 16)
        {
            elements = new JMbr[capacity];
            count = 0;
            current = -1;
        }

        public JArr(params int[] elems) : this(elems.Length)
        {
            for (int i = 0; i < elems.Length; i++)
            {
                Add(new JMbr(null, elems[i]));
            }
        }

        public JArr(params string[] elems) : this(elems.Length)
        {
            for (int i = 0; i < elems.Length; i++)
            {
                Add(new JMbr(null, elems[i]));
            }
        }

        public JMbr this[int index] => elements[index];

        public int Count => count;

        internal void Add(JMbr e)
        {
            int len = elements.Length;
            if (count >= len)
            {
                JMbr[] alloc = new JMbr[len * 4];
                Array.Copy(elements, 0, alloc, 0, len);
                elements = alloc;
            }
            elements[count++] = e;
        }

        public bool Get(string name, ref bool v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
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

        public bool Get<D>(string name, ref D v, int proj = 0) where D : IData, new()
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v, proj);
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

        public bool Get<D>(string name, ref D[] v, int proj = 0) where D : IData, new()
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get<D>(string name, ref List<D> v, int proj = 0) where D : IData, new()
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public D ToObject<D>(int proj = 0) where D : IData, new()
        {
            D obj = new D();
            obj.ReadData(this, proj);
            return obj;
        }

        public D[] ToArray<D>(int proj = 0) where D : IData, new()
        {
            D[] arr = new D[count];
            for (int i = 0; i < arr.Length; i++)
            {
                D obj = new D();
                obj.ReadData((JObj)elements[i], proj);
                arr[i] = obj;
            }
            return arr;
        }

        public List<D> ToList<D>(int proj = 0) where D : IData, new()
        {
            List<D> lst = new List<D>(count + 8);
            for (int i = 0; i < count; i++)
            {
                D obj = new D();
                obj.ReadData((JObj)elements[i], proj);
                lst.Add(obj);
            }
            return lst;
        }

        public void WriteData<R>(IDataOutput<R> o) where R : IDataOutput<R>
        {
            for (int i = 0; i < count; i++)
            {
                JMbr mbr = elements[i];
                JType t = mbr.type;
                if (t == JType.Array)
                {
                    o.Put(null, (JArr)mbr);
                }
                else if (t == JType.Object)
                {
                    o.Put(null, (JObj)mbr);
                }
                else if (t == JType.String)
                {
                    o.Put(null, (string)mbr);
                }
                else if (t == JType.Number)
                {
                    o.Put(null, (JNumber)mbr);
                }
                else if (t == JType.True)
                {
                    o.Put(null, true);
                }
                else if (t == JType.False)
                {
                    o.Put(null, false);
                }
                else if (t == JType.Null)
                {
                    o.PutNull(null);
                }
            }
        }

        public IContent Dump()
        {
            var cont = new JsonContent(true, true);
            cont.Put(null, this);
            return cont;
        }

        public bool DataSet => true;

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
    }
}