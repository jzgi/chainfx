using System;

namespace Greatbone.Core
{
    ///
    /// A JSON object model.
    ///
    public class Obj : Roll<Member>, ISource
    {
        const int InitialCapacity = 16;

        public Obj(int capacity = InitialCapacity) : base(capacity)
        {
        }

        /// To add null value
        internal void AddNull(string name)
        {
            Add(new Member(name, (Member?)null));
        }

        internal void Add(string name, Obj v)
        {
            Add(new Member(name, v));
        }

        internal void Add(string name, Arr v)
        {
            Add(new Member(name, v));
        }

        internal void Add(string name, string v)
        {
            Add(new Member(name, v));
        }

        internal void Add(string name, byte[] v)
        {
            Add(new Member(name, v));
        }

        internal void Add(string name, bool v)
        {
            Add(new Member(name, v));
        }

        internal void Add(string name, Number v)
        {
            Add(new Member(name, v));
        }

        //
        // SOURCE
        //

        public bool Get(string name, ref bool v)
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref Number v)
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref char[] v)
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref ArraySegment<byte>? v)
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                byte[] bv = pair;
                v = new ArraySegment<byte>(bv);
                return true;
            }
            return false;
        }

        public bool Get<D>(string name, ref D v, byte z = 0) where D : IDat, new()
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                Obj obj = pair;
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
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref Arr v)
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                v = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short[] v)
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                Arr arr = pair;
                if (arr != null)
                {
                    v = new short[arr.Count];
                    for (int i = 0; i < arr.Count; i++)
                    {
                        v[i] = arr[i];
                    }
                }
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int[] v)
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                Arr arr = pair;
                if (arr != null)
                {
                    v = new int[arr.Count];
                    for (int i = 0; i < arr.Count; i++)
                    {
                        v[i] = arr[i];
                    }
                }
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long[] v)
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                Arr arr = pair;
                if (arr != null)
                {
                    v = new long[arr.Count];
                    for (int i = 0; i < arr.Count; i++)
                    {
                        v[i] = arr[i];
                    }
                }
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string[] v)
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                Arr arr = pair;
                if (arr != null)
                {
                    v = new string[arr.Count];
                    for (int i = 0; i < arr.Count; i++)
                    {
                        v[i] = arr[i];
                    }
                }
                return true;
            }
            return false;
        }

        public bool Get<D>(string name, ref D[] v, byte z = 0) where D : IDat, new()
        {
            Member pair;
            if (TryGet(name, out pair))
            {
                Arr arr = pair;
                if (arr != null)
                {
                    v = new D[arr.Count];
                    for (int i = 0; i < arr.Count; i++)
                    {
                        Obj obj = arr[i];
                        D dat = new D();
                        dat.Load(obj);
                        v[i] = dat;
                    }
                }
                return true;
            }
            return false;
        }


        internal void Dump<R>(ISink<R> snk) where R : ISink<R>
        {
            for (int i = 0; i < Count; i++)
            {
                Member mbr = this[i];
                MemberType typ = mbr.type;
                if (typ == MemberType.Array)
                {
                    snk.Put(mbr.Name, (Arr)mbr);
                }
                else if (typ == MemberType.Object)
                {
                    snk.Put(mbr.Name, (Obj)mbr);
                }
                else if (typ == MemberType.String)
                {
                    snk.Put(mbr.Name, (string)mbr);
                }
                else if (typ == MemberType.Number)
                {
                    snk.Put(mbr.Name, (Number)mbr);
                }
                else if (typ == MemberType.True)
                {
                    snk.Put(mbr.Name, true);
                }
                else if (typ == MemberType.False)
                {
                    snk.Put(mbr.Name, false);
                }
                else if (typ == MemberType.Null)
                {
                    snk.PutNull(mbr.Name);
                }
            }
        }
    }
}