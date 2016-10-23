using System;

namespace Greatbone.Core
{
    /// <summary>
    /// An object JSON data model.
    /// </summary>
    public class JObj : ISource
    {
        const int InitialCapacity = 16;

        readonly Roll<JMember> pairs;

        public JObj(int capacity = InitialCapacity)
        {
            pairs = new Roll<JMember>(16);
        }

        internal void Add(string name)
        {
            JMember e = new JMember()
            {
                Key = name
            };
            pairs.Add(e);
        }

        internal void Add(string name, JObj v)
        {
            JMember e = new JMember(v)
            {
                Key = name
            };
            pairs.Add(e);
        }

        internal void Add(string name, JArr v)
        {
            JMember e = new JMember(v)
            {
                Key = name
            };
            pairs.Add(e);
        }

        internal void Add(string name, string v)
        {
            JMember e = new JMember(v)
            {
                Key = name
            };
            pairs.Add(e);
        }

        internal void Add(string name, byte[] v)
        {
            JMember e = new JMember(v)
            {
                Key = name
            };
            pairs.Add(e);
        }

        internal void Add(string name, bool v)
        {
            JMember e = new JMember(v)
            {
                Key = name
            };
            pairs.Add(e);
        }

        internal void Add(string name, Number v)
        {
            JMember e = new JMember(v)
            {
                Key = name
            };
            pairs.Add(e);
        }

        public int Count => pairs.Count;

        public JMember this[int index] => pairs[index];

        public JMember this[string name] => pairs[name];

        //
        // SOURCE
        //

        public bool Got(string name, ref bool v)
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (bool)pair;
                return true;
            }
            return false;
        }

        public bool Got(string name, ref short v)
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (short)pair;
                return true;
            }
            return false;
        }

        public bool Got(string name, ref int v)
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (int)pair;
                return true;
            }
            return false;
        }

        public bool Got(string name, ref long v)
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (short)pair;
                return true;
            }
            return false;
        }

        public bool Got(string name, ref decimal v)
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (decimal)pair;
                return true;
            }
            return false;
        }

        public bool Got(string name, ref Number v)
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (Number)pair;
                return true;
            }
            return false;
        }

        public bool Got(string name, ref DateTime v)
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (DateTime)pair;
                return true;
            }
            return false;
        }

        public bool Got(string name, ref char[] v)
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (char[])pair;
                return true;
            }
            return false;
        }

        public bool Got(string name, ref string v)
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (string)pair;
                return true;
            }
            return false;
        }

        public bool Got(string name, ref byte[] v)
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (byte[])pair;
                return true;
            }
            return false;
        }

        public bool Got(string name, ref ArraySegment<byte> v)
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                byte[] bv = (byte[])pair;
                v = new ArraySegment<byte>(bv);
                return true;
            }
            return false;
        }

        public bool Got<P>(string name, ref P v, uint x = 0) where P : IPersist, new()
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                JObj jo = (JObj)pair;
                v = new P(); v.Load(jo);
                return true;
            }
            return false;
        }

        public bool Got(string name, ref JObj v)
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (JObj)pair;
                return true;
            }
            return false;
        }

        public bool Got(string name, ref JArr v)
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                v = (JArr)pair;
                return true;
            }
            return false;
        }

        public bool Got(string name, ref short[] v)
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                JArr ja = pair;
                v = new short[ja.Count];
                for (int i = 0; i < ja.Count; i++)
                {
                    v[i] = ja[i];
                }
                return true;
            }
            return false;
        }

        public bool Got(string name, ref int[] v)
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                JArr ja = pair;
                v = new int[ja.Count];
                for (int i = 0; i < ja.Count; i++)
                {
                    v[i] = ja[i];
                }
                return true;
            }
            return false;
        }

        public bool Got(string name, ref long[] v)
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                JArr ja = pair;
                v = new long[ja.Count];
                for (int i = 0; i < ja.Count; i++)
                {
                    v[i] = ja[i];
                }
                return true;
            }
            return false;
        }

        public bool Got(string name, ref string[] v)
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                JArr ja = pair;
                v = new string[ja.Count];
                for (int i = 0; i < ja.Count; i++)
                {
                    v[i] = ja[i];
                }
                return true;
            }
            return false;
        }

        public bool Got<P>(string name, ref P[] v, uint x = 0) where P : IPersist, new()
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                JArr ja = pair;
                v = new P[ja.Count];
                for (int i = 0; i < ja.Count; i++)
                {
                    JObj jo = ja[i];
                    P obj = new P(); obj.Load(jo);
                    v[i] = obj;
                }
                return true;
            }
            return false;
        }


        internal void Save<R>(ISink<R> sk) where R : ISink<R>
        {
            for (int i = 0; i < pairs.Count; i++)
            {
                JMember pair = pairs[i];
                VT vt = pair.vt;
                if (vt == VT.Array)
                {
                    sk.Put(pair.Key, (JArr)pair);
                }
                else if (vt == VT.Object)
                {
                    sk.Put(pair.Key, (JObj)pair);
                }
                else if (vt == VT.String)
                {
                    sk.Put(pair.Key, (string)pair);
                }
                else if (vt == VT.Number)
                {
                    sk.Put(pair.Key, (Number)pair);
                }
                else if (vt == VT.True)
                {
                    sk.Put(pair.Key, true);
                }
                else if (vt == VT.False)
                {
                    sk.Put(pair.Key, false);
                }
                else if (vt == VT.Null)
                {
                    sk.PutNull(pair.Key);
                }
            }
        }

    }
}