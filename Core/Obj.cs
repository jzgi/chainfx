using System;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// A JSON object model.
    /// </summary>
    ///
    public class Obj : ISource
    {
        const int InitialCapacity = 16;

        readonly Roll<Member> pairs;

        public Obj(int capacity = InitialCapacity)
        {
            pairs = new Roll<Member>(16);
        }

        // add null
        internal void Add(string name)
        {
            pairs.Add(new Member() { Key = name });
        }

        internal void Add(string name, Obj v)
        {
            pairs.Add(new Member(v) { Key = name });
        }

        internal void Add(string name, Arr v)
        {
            pairs.Add(new Member(v) { Key = name });
        }

        internal void Add(string name, string v)
        {
            pairs.Add(new Member(v) { Key = name });
        }

        internal void Add(string name, byte[] v)
        {
            pairs.Add(new Member(v) { Key = name });
        }

        internal void Add(string name, bool v)
        {
            pairs.Add(new Member(v) { Key = name });
        }

        internal void Add(string name, Number v)
        {
            pairs.Add(new Member(v) { Key = name });
        }

        public int Count => pairs.Count;

        public Member this[int index] => pairs[index];

        public Member this[string name] => pairs[name];

        //
        // SOURCE
        //

        public bool Get(string name, ref bool v)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (bool)pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (short)pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (int)pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (short)pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (decimal)pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref Number v)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (Number)pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (DateTime)pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref char[] v)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (char[])pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (string)pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (byte[])pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref ArraySegment<byte>? v)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                byte[] bv = (byte[])pair;
                v = new ArraySegment<byte>(bv);
                return true;
            }
            return false;
        }

        public bool Get<D>(string name, ref D v, byte z = 0) where D : IBean, new()
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                Obj obj = (Obj)pair;
                if (obj != null)
                {
                    v = new D();
                    v.Load(obj);
                }
                return true;
            }
            return false;
        }

        public bool Get(string name, ref Obj v)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (Obj)pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref Arr v)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (Arr)pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short[] v)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                Arr ja = pair;
                if (ja != null)
                {
                    v = new short[ja.Count];
                    for (int i = 0; i < ja.Count; i++)
                    {
                        v[i] = ja[i];
                    }
                }
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int[] v)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                Arr ja = pair;
                if (ja != null)
                {
                    v = new int[ja.Count];
                    for (int i = 0; i < ja.Count; i++)
                    {
                        v[i] = ja[i];
                    }
                }
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long[] v)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                Arr ja = pair;
                if (ja != null)
                {
                    v = new long[ja.Count];
                    for (int i = 0; i < ja.Count; i++)
                    {
                        v[i] = ja[i];
                    }
                }
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string[] v)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                Arr ja = pair;
                if (ja != null)
                {
                    v = new string[ja.Count];
                    for (int i = 0; i < ja.Count; i++)
                    {
                        v[i] = ja[i];
                    }
                }
                return true;
            }
            return false;
        }

        public bool Get<D>(string name, ref D[] v, byte z = 0) where D : IBean, new()
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                Arr arr = pair;
                if (arr != null)
                {
                    v = new D[arr.Count];
                    for (int i = 0; i < arr.Count; i++)
                    {
                        Obj obj = arr[i];
                        D dat = new D(); dat.Load(obj);
                        v[i] = dat;
                    }
                }
                return true;
            }
            return false;
        }


        internal void Dump<R>(ISink<R> snk) where R : ISink<R>
        {
            for (int i = 0; i < pairs.Count; i++)
            {
                Member mbr = pairs[i];
                MemberType typ = mbr.type;
                if (typ == MemberType.Array)
                {
                    snk.Put(mbr.Key, (Arr)mbr);
                }
                else if (typ == MemberType.Object)
                {
                    snk.Put(mbr.Key, (Obj)mbr);
                }
                else if (typ == MemberType.String)
                {
                    snk.Put(mbr.Key, (string)mbr);
                }
                else if (typ == MemberType.Number)
                {
                    snk.Put(mbr.Key, (Number)mbr);
                }
                else if (typ == MemberType.True)
                {
                    snk.Put(mbr.Key, true);
                }
                else if (typ == MemberType.False)
                {
                    snk.Put(mbr.Key, false);
                }
                else if (typ == MemberType.Null)
                {
                    snk.PutNull(mbr.Key);
                }
            }
        }

    }

}