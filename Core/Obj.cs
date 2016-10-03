using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class Obj : ISource
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

        public int Count => pairs.Count;

        public Member this[int index] => pairs[index];

        public Member this[string name] => pairs[name];

        //
        // SOURCE
        //

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

        public bool Get<T>(string name, ref T value, int x = -1) where T : IPersist, new()
        {
            throw new NotImplementedException();
        }

        public bool Get<T>(string name, ref List<T> value, int x = -1) where T : IPersist, new()
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref byte[] value)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref Obj value)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref Arr value)
        {
            throw new NotImplementedException();
        }

    }
}