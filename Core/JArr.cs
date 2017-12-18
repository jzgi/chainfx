using System;
using System.Collections;

namespace Greatbone.Core
{
    /// <summary>
    /// A JSON array model.
    /// </summary>
    public class JArr : IDataInput, IEnumerable
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

        public JArr(params int[] values) : this(values.Length)
        {
            for (int i = 0; i < values.Length; i++)
            {
                Add(new JMbr(values[i]));
            }
        }

        public JArr(params string[] values) : this(values.Length)
        {
            for (int i = 0; i < values.Length; i++)
            {
                Add(new JMbr(values[i]));
            }
        }

        public JMbr this[int index] => elements[index];

        public int Count => count;

        /// <summary>
        /// This add can be used in initialzier
        /// </summary>
        /// <param name="elem"></param>
        internal void Add(JMbr elem)
        {
            int len = elements.Length;
            if (count >= len)
            {
                JMbr[] alloc = new JMbr[len * 4];
                Array.Copy(elements, 0, alloc, 0, len);
                elements = alloc;
            }
            elements[count++] = elem;
        }

        public void Add(JObj elem)
        {
            Add(new JMbr(elem));
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

        public bool Get(string name, ref string v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get<D>(string name, ref D v, short proj = 0x00ff) where D : IData, new()
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

        public bool Get(string name, ref Map<string, string> v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D[] v, short proj = 0x00ff) where D : IData, new()
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        //
        // LET
        //

        public IDataInput Let(out bool v)
        {
            v = elements[current];
            return this;
        }

        public IDataInput Let(out short v)
        {
            v = elements[current];
            return this;
        }

        public IDataInput Let(out int v)
        {
            v = elements[current];
            return this;
        }

        public IDataInput Let(out long v)
        {
            v = elements[current];
            return this;
        }

        public IDataInput Let(out double v)
        {
            v = elements[current];
            return this;
        }

        public IDataInput Let(out decimal v)
        {
            v = elements[current];
            return this;
        }

        public IDataInput Let(out DateTime v)
        {
            v = elements[current];
            return this;
        }

        public IDataInput Let(out string v)
        {
            v = elements[current];
            return this;
        }

        public IDataInput Let(out ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out short[] v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out int[] v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out long[] v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out string[] v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out Map<string, string> v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let<D>(out D v, short proj = 0x00ff) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public IDataInput Let<D>(out D[] v, short proj = 0x00ff) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        //
        // ENTIRITY
        //

        public D ToObject<D>(short proj = 0x00ff) where D : IData, new()
        {
            D obj = new D();
            obj.Read(this, proj);
            return obj;
        }

        public D[] ToArray<D>(short proj = 0x00ff) where D : IData, new()
        {
            D[] arr = new D[count];
            for (int i = 0; i < arr.Length; i++)
            {
                D obj = new D();
                obj.Read((JObj) elements[i], proj);
                arr[i] = obj;
            }
            return arr;
        }

        public Map<K, D> ToMap<K, D>(Func<D, K> keyer, short proj = 0x00ff) where D : IData, new()
        {
            Map<K, D> coll = new Map<K, D>();
            for (int i = 0; i < count; i++)
            {
                D obj = new D();
                obj.Read((JObj) elements[i], proj);
                K key = keyer(obj);
                coll.Add(key, obj);
            }
            return coll;
        }

        public void Write<R>(IDataOutput<R> o) where R : IDataOutput<R>
        {
            for (int i = 0; i < count; i++)
            {
                JMbr elem = elements[i];
                JType t = elem.type;
                if (t == JType.Array)
                {
                    o.Put(null, (IDataInput) (JArr) elem);
                }
                else if (t == JType.Object)
                {
                    o.Put(null, (JObj) elem);
                }
                else if (t == JType.String)
                {
                    o.Put(null, (string) elem);
                }
                else if (t == JType.Number)
                {
                    o.Put(null, (JNumber) elem);
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

        public DynamicContent Dump()
        {
            var cont = new JsonContent(true);
            cont.Put(null, (IDataInput) this);
            return cont;
        }

        public bool DataSet => true;

        public bool Next()
        {
            return ++current < count;
        }

        public override string ToString()
        {
            JsonContent cont = new JsonContent(false);
            cont.Put(null, (IDataInput) this);
            string str = cont.ToString();
            BufferUtility.Return(cont);
            return str;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public static implicit operator string[](JArr v)
        {
            string[] arr = new string[v.count];
            for (int i = 0; i < v.count; i++)
            {
                arr[i] = v[i];
            }
            return arr;
        }
    }
}