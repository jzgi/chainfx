using System;

namespace Greatbone.Core
{
    ///
    /// Represents a value or a name/value pair if with the name.
    ///
    public struct Member : IRollable
    {
        // property name, if not null
        readonly string name;

        // type of the value
        internal readonly MemberType type;

        // Obj, Arr, string, byte[]
        internal readonly object refv;

        internal readonly Number numv;

        public Member(string name, Member? v) : this(v)
        {
            this.name = name;
        }

        public Member(string name, Obj v) : this(v)
        {
            this.name = name;
        }

        public Member(string name, Arr v) : this(v)
        {
            this.name = name;
        }

        public Member(string name, string v) : this(v)
        {
            this.name = name;
        }

        public Member(string name, byte[] v) : this(v)
        {
            this.name = name;
        }

        public Member(string name, bool v) : this(v)
        {
            this.name = name;
        }

        public Member(string name, Number v) : this(v)
        {
            this.name = name;
        }

        public Member(Member? v)
        {
            name = null;
            type = MemberType.Null;
            refv = null;
            numv = default(Number);
        }

        public Member(Obj v)
        {
            name = null;
            type = MemberType.Object;
            refv = v;
            numv = default(Number);
        }

        public Member(Arr v)
        {
            name = null;
            type = MemberType.Array;
            refv = v;
            numv = default(Number);
        }

        public Member(string v)
        {
            name = null;
            type = MemberType.String;
            refv = v;
            numv = default(Number);
        }

        public Member(byte[] v)
        {
            name = null;
            type = MemberType.Bytes;
            refv = v;
            numv = default(Number);
        }

        public Member(bool v)
        {
            name = null;
            type = v ? MemberType.True : MemberType.False;
            refv = null;
            numv = default(Number);
        }

        public Member(Number v)
        {
            name = null;
            type = MemberType.Number;
            refv = null;
            numv = v;
        }


        public string Name => name;

        public bool IsPair => name != null;

        public static implicit operator Obj(Member v)
        {
            if (v.type == MemberType.Object)
            {
                return (Obj)v.refv;
            }
            return null;
        }

        public static implicit operator Arr(Member v)
        {
            if (v.type == MemberType.Array)
            {
                return (Arr)v.refv;
            }
            return null;
        }

        public static implicit operator bool(Member v)
        {
            return v.type == MemberType.True;
        }

        public static implicit operator short(Member v)
        {
            if (v.type == MemberType.Number)
            {
                return v.numv.Short;
            }
            return 0;
        }

        public static implicit operator int(Member v)
        {
            if (v.type == MemberType.Number)
            {
                return v.numv.Int;
            }
            return 0;
        }

        public static implicit operator long(Member v)
        {
            if (v.type == MemberType.Number)
            {
                return v.numv.Long;
            }
            return 0;
        }

        public static implicit operator decimal(Member v)
        {
            if (v.type == MemberType.Number)
            {
                return v.numv.Decimal;
            }
            return 0;
        }

        public static implicit operator Number(Member v)
        {
            if (v.type == MemberType.Number)
            {
                return v.numv;
            }
            return default(Number);
        }

        public static implicit operator DateTime(Member v)
        {
            if (v.type == MemberType.String)
            {
                string str = (string)v.refv;
                DateTime dt;
                if (StrUtility.TryParseDate(str, out dt)) return dt;
            }
            return default(DateTime);
        }

        public static implicit operator char[] (Member v)
        {
            if (v.type == MemberType.String)
            {
                string str = (string)v.refv;
                return str.ToCharArray();
            }
            return null;
        }

        public static implicit operator string(Member v)
        {
            if (v.type == MemberType.String)
            {
                return (string)v.refv;
            }
            return null;
        }

        public static implicit operator byte[] (Member v)
        {
            if (v.type == MemberType.String)
            {
                return (byte[])v.refv;
            }
            return null;
        }
    }
}