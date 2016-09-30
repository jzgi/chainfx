using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class Obj : IIn
    {
        readonly Roll<Member> pairs = new Roll<Member>(16);

        internal void Add(string name)
        {
            Member e = new Member()
            {
                Key = name
            };
            pairs.Add(e);
        }

        internal void Add(string name, Obj v)
        {
            Member e = new Member(v)
            {
                Key = name
            };
            pairs.Add(e);
        }

        internal void Add(string name, Arr v)
        {
            Member e = new Member(v)
            {
                Key = name
            };
            pairs.Add(e);
        }

        internal void Add(string name, string v)
        {
            Member e = new Member(v)
            {
                Key = name
            };
            pairs.Add(e);
        }

        internal void Add(string name, byte[] v)
        {
            Member e = new Member(v)
            {
                Key = name
            };
            pairs.Add(e);
        }

        internal void Add(string name, bool v)
        {
            Member e = new Member(v)
            {
                Key = name
            };
            pairs.Add(e);
        }

        internal void Add(string name, Number v)
        {
            Member e = new Member(v)
            {
                Key = name
            };
            pairs.Add(e);
        }


        public bool Get(string name, ref int value)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                value = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref bool value)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                value = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short value)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                value = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long value)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                value = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal value)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                value = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime value)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                value = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref char[] value)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                value = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string value)
        {
            Member pair;
            if (pairs.TryGet(name, out pair))
            {
                value = pair;
                return true;
            }
            return false;
        }

        public bool Get<T>(string name, ref T value) where T : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref byte[] value)
        {
            throw new NotImplementedException();
        }

        public bool Get<T>(string name, ref List<T> value)
        {
            throw new NotImplementedException();
        }

        public bool Get<T>(string name, ref Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }

        public Member this[string name]
        {
            get
            {
                return pairs[name];
            }
        }
    }
}