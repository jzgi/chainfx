using System;

namespace Greatbone.Core
{
    ///
    /// Represents a value or a name/value pair if with the name.
    ///
    public struct Member : IKeyed
    {
        // type of the value
        internal readonly MemberType type;

        // Obj, Arr, string, byte[]
        internal readonly object refv;

        internal readonly Number numv;

        // key as in an object member
        string key;

        internal Member(Obj v)
        {
            type = MemberType.Object;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal Member(Arr v)
        {
            type = MemberType.Array;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal Member(string v)
        {
            type = MemberType.String;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal Member(byte[] v)
        {
            type = MemberType.Bytes;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal Member(bool v)
        {
            type = v ? MemberType.True : MemberType.False;
            refv = null;
            numv = default(Number);
            key = null;
        }

        internal Member(Number v)
        {
            type = MemberType.Number;
            refv = null;
            numv = v;
            key = null;
        }


        public string Key
        {
            get { return key; }
            internal set { key = value; }
        }

        public bool IsPair => key != null;

        public static implicit operator Obj(Member v)
        {
            if (v.type == MemberType.Object)
            {
                return (Obj) v.refv;
            }
            return null;
        }

        public static implicit operator Arr(Member v)
        {
            if (v.type == MemberType.Array)
            {
                return (Arr) v.refv;
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
                string str = (string) v.refv;
                DateTime dt;
                if (StrUtility.TryParseDate(str, out dt)) return dt;
            }
            return default(DateTime);
        }

        public static implicit operator char[](Member v)
        {
            if (v.type == MemberType.String)
            {
                string str = (string) v.refv;
                return str.ToCharArray();
            }
            return null;
        }

        public static implicit operator string(Member v)
        {
            if (v.type == MemberType.String)
            {
                return (string) v.refv;
            }
            return null;
        }

        public static implicit operator byte[](Member v)
        {
            if (v.type == MemberType.String)
            {
                return (byte[]) v.refv;
            }
            return null;
        }
    }
}