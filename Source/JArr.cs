using System;
using System.Collections;

namespace Skyiah
{
    /// <summary>
    /// A JSON array model.
    /// </summary>
    public class JArr : ISource, IEnumerable
    {
        // array elements
        JMbr[] elements;

        int count;

        int current;

        public JArr(int capacity = 16)
        {
            elements = new JMbr[capacity];
            count = 0;
            current = -1;
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

        public void Add(JArr elem)
        {
            Add(new JMbr(elem));
        }

        public void Add(bool elem)
        {
            Add(new JMbr(elem));
        }

        public void Add(JNumber elem)
        {
            Add(new JMbr(elem));
        }

        public bool Get(string name, ref bool v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref char v)
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

        public bool Get(string name, ref byte[] v)
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get<D>(string name, ref D v, byte proj = 0x0f) where D : IData, new()
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

        public bool Get(string name, ref JObj v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref JArr v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D[] v, byte proj = 0x0f) where D : IData, new()
        {
            JObj jo = elements[current];
            return jo != null && jo.Get(name, ref v);
        }

        //
        // ENTIRITY
        //

        public D ToObject<D>(byte proj = 0x0f) where D : IData, new()
        {
            D obj = new D();
            obj.Read(this, proj);
            return obj;
        }

        public D[] ToArray<D>(byte proj = 0x0f) where D : IData, new()
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

        public Map<K, D> ToMap<K, D>(byte proj = 0x0f, Func<D, K> keyer = null, Predicate<K> toper = null) where D : IData, new()
        {
            Map<K, D> map = new Map<K, D>();
            for (int i = 0; i < count; i++)
            {
                D obj = new D();
                obj.Read((JObj) elements[i], proj);
                K key = default;
                if (keyer != null)
                {
                    key = keyer(obj);
                }
                else if (obj is IKeyable<K> mappable)
                {
                    key = mappable.Key;
                }

                map.Add(key, obj);
            }

            return map;
        }

        public bool IsDataSet => true;

        public bool Next()
        {
            return ++current < count;
        }

        public void Write<C>(C cnt) where C : IContent, ISink
        {
            for (int i = 0; i < count; i++)
            {
                JMbr e = elements[i];
                JType t = e.type;
                if (t == JType.Array)
                {
                    cnt.Put(null, (JArr) e);
                }
                else if (t == JType.Object)
                {
                    cnt.Put(null, (JObj) e);
                }
                else if (t == JType.String)
                {
                    cnt.Put(null, (string) e);
                }
                else if (t == JType.Number)
                {
                    cnt.Put(null, (JNumber) e);
                }
                else if (t == JType.True)
                {
                    cnt.Put(null, true);
                }
                else if (t == JType.False)
                {
                    cnt.Put(null, false);
                }
                else if (t == JType.Null)
                {
                    cnt.PutNull(null);
                }
            }
        }

        public IContent Dump()
        {
            var cnt = new JsonContent(4096);
            cnt.PutFromSource(this);
            return cnt;
        }

        public override string ToString()
        {
            var cnt = new JsonContent(4 * 1024, binary: false);
            try
            {
                cnt.Put(null, this);
                return cnt.ToString();
            }
            finally
            {
                ArrayUtility.Return(cnt.Buffer); // return buffer to pool
            }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public static implicit operator string[](JArr v)
        {
            var arr = new string[v.count];
            for (int i = 0; i < v.count; i++)
            {
                arr[i] = v[i];
            }

            return arr;
        }
    }
}