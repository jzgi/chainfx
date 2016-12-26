using System;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// Represents a value or a property if with the name.
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

        public Member(string name)
        {
            this.name = name;
            type = MemberType.Null;
            refv = null;
            numv = default(Number);
        }

        public Member(string name, Obj v)
        {
            this.name = name;
            type = MemberType.Object;
            refv = v;
            numv = default(Number);
        }

        public Member(string name, Arr v)
        {
            this.name = name;
            type = MemberType.Array;
            refv = v;
            numv = default(Number);
        }

        public Member(string name, string v)
        {
            this.name = name;
            type = MemberType.String;
            refv = v;
            numv = default(Number);
        }

        public Member(string name, byte[] v)
        {
            this.name = name;
            type = MemberType.Bytes;
            refv = v;
            numv = default(Number);
        }

        public Member(string name, bool v)
        {
            this.name = name;
            type = v ? MemberType.True : MemberType.False;
            refv = null;
            numv = default(Number);
        }

        public Member(string name, Number v)
        {
            this.name = name;
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

        public static implicit operator double(Member v)
        {
            if (v.type == MemberType.Number)
            {
                return v.numv.Double;
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

        public static implicit operator NpgsqlPoint(Member v)
        {
            if (v.type == MemberType.String)
            {
                string str = (string) v.refv;
                if (str != null)
                {
                    int comma = str.IndexOf(',');
                    if (comma != -1)
                    {
                        string xstr = str.Substring(comma);
                        string ystr = str.Substring(comma + 1);
                        double x, y;
                        if (double.TryParse(xstr, out x) && double.TryParse(ystr, out y))
                        {
                            return new NpgsqlPoint(x, y);
                        }
                    }
                }
            }
            return default(NpgsqlPoint);
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