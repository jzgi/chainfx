using System;
using System.Collections;

namespace ChainFx
{
    /// <summary>
    /// A JSON array.
    /// </summary>
    public class JArr : ISource, IEnumerable
    {
        // array elements
        JMbr[] elements;

        int count;

        // current index while looping
        int cur;

        public JArr(int capacity = 16)
        {
            elements = new JMbr[capacity];
            count = 0;
            cur = -1;
        }

        public JMbr this[int index] => elements[index];

        public int Count => count;

        internal void ResetCur()
        {
            cur = -1;
        }

        /// <summary>
        /// This add can be used in initialzier
        /// </summary>
        internal void Add(JMbr el)
        {
            int len = elements.Length;
            if (count >= len)
            {
                var alloc = new JMbr[len * 4];
                Array.Copy(elements, 0, alloc, 0, len);
                elements = alloc;
            }
            elements[count++] = el;
        }

        public void Add(JObj el)
        {
            Add(new JMbr(el));
        }

        public void Add(JArr el)
        {
            Add(new JMbr(el));
        }

        public void Add(bool el)
        {
            Add(new JMbr(el));
        }

        public void Add(JNumber el)
        {
            Add(new JMbr(el));
        }

        public bool Get(string name, ref bool v)
        {
            JObj jo = elements[cur];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref char v)
        {
            JObj jo = elements[cur];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref short v)
        {
            JObj jo = elements[cur];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref int v)
        {
            JObj jo = elements[cur];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref long v)
        {
            JObj jo = elements[cur];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref double v)
        {
            JObj jo = elements[cur];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref decimal v)
        {
            JObj jo = elements[cur];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref DateTime v)
        {
            JObj jo = elements[cur];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref Guid v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref string v)
        {
            JObj jo = elements[cur];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            JObj jo = elements[cur];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref byte[] v)
        {
            JObj jo = elements[cur];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get<D>(string name, ref D v, short msk = 0xff) where D : IData, new()
        {
            JObj jo = elements[cur];
            return jo != null && jo.Get(name, ref v, msk);
        }

        public bool Get(string name, ref short[] v)
        {
            JObj jo = elements[cur];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref int[] v)
        {
            JObj jo = elements[cur];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref long[] v)
        {
            JObj jo = elements[cur];
            return jo != null && jo.Get(name, ref v);
        }

        public bool Get(string name, ref string[] v)
        {
            JObj jo = elements[cur];
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


        public bool Get(string name, ref XElem v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D[] v, short msk = 0xff) where D : IData, new()
        {
            JObj jo = elements[cur];
            return jo != null && jo.Get(name, ref v);
        }

        //
        // ENTIRITY
        //

        public D ToObject<D>(short msk = 0xff) where D : IData, new()
        {
            var obj = new D();
            obj.Read(this, msk);
            return obj;
        }

        public D[] ToArray<D>(short msk = 0xff) where D : IData, new()
        {
            var arr = new D[count];
            for (int i = 0; i < arr.Length; i++)
            {
                var obj = new D();
                obj.Read((JObj) elements[i], msk);
                arr[i] = obj;
            }

            return arr;
        }

        public Map<K, D> ToMap<K, D>(short proj = 0xff, Func<D, K> keyer = null, Predicate<K> toper = null) where D : IData, new()
        {
            var map = new Map<K, D>();
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
            return ++cur < count;
        }

        public void Write<C>(C cnt) where C : ContentBuilder, ISink
        {
            for (int i = 0; i < count; i++)
            {
                var el = elements[i];
                var typ = el.typ;
                if (typ == JType.Array)
                {
                    cnt.Put(null, (JArr) el);
                }
                else if (typ == JType.Object)
                {
                    cnt.Put(null, (JObj) el);
                }
                else if (typ == JType.String)
                {
                    cnt.Put(null, (string) el);
                }
                else if (typ == JType.Number)
                {
                    cnt.Put(null, (JNumber) el);
                }
                else if (typ == JType.True)
                {
                    cnt.Put(null, true);
                }
                else if (typ == JType.False)
                {
                    cnt.Put(null, false);
                }
                else if (typ == JType.Null)
                {
                    cnt.PutNull(null);
                }
            }
        }

        public IContent Dump()
        {
            var cnt = new JsonBuilder(true, 4096);
            cnt.PutFromSource(this);
            return cnt;
        }

        public override string ToString()
        {
            var cnt = new JsonBuilder(false, 4 * 1024);
            try
            {
                cnt.Put(null, this);
                return cnt.ToString();
            }
            finally
            {
                cnt.Dispose();
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