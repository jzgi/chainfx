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

        public bool Get(string name, ref Dictionary<string, string> v)
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

        public D ToObject<D>(byte flags = 0) where D : IData, new()
        {
            D obj = new D();
            obj.ReadData(this, flags);
            return obj;
        }

        public D[] ToArray<D>(byte flags = 0) where D : IData, new()
        {
            D[] arr = new D[count];
            for (int i = 0; i < arr.Length; i++)
            {
                D obj = new D();
                obj.ReadData((JObj)elements[i], flags);
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
                obj.ReadData((JObj)elements[i], flags);
                lst.Add(obj);
            }
            return lst;
        }

        public void WriteData<R>(IDataOutput<R> o) where R : IDataOutput<R>
        {
            for (int i = 0; i < count; i++)
            {
                JMem mem = elements[i];
                JType t = mem.type;
                if (t == JType.Array)
                {
                    o.Put(null, (JArr)mem);
                }
                else if (t == JType.Object)
                {
                    o.Put(null, (JObj)mem);
                }
                else if (t == JType.String)
                {
                    o.Put(null, (string)mem);
                }
                else if (t == JType.Number)
                {
                    o.Put(null, (JNumber)mem);
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

        public C Dump<C>() where C : IContent, IDataOutput<C>, new()
        {
            C cont = new C();
            WriteData(cont);
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

        public bool Get(string name, ref IDataInput v)
        {
            throw new NotImplementedException();
        }
    }
}