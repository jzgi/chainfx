using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// An object or record data model.
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

        public bool Got<T>(string name, ref T v) where T : IPersist, new()
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                T val = new T();
                val.Load((JObj)pair);
                v = val;
                return true;
            }
            return false;
        }

        public bool Got<T>(string name, ref List<T> v) where T : IPersist, new()
        {
            JMember pair;
            if (pairs.TryGet(name, out pair))
            {
                JArr ma = pair;
                List<T> lst = null;
                for (int i = 0; i < ma.Count; i++)
                {
                    JObj el = ma[i];
                    T val = new T();
                    val.Load(el);
                    if (lst == null) lst = new List<T>(16);
                    lst.Add(val);
                }
                v = lst;
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

    }
}