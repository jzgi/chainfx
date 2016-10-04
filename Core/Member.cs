using System;

namespace Greatbone.Core
{

    /// <summary>
    /// The enumeration of value types.
    /// </summary>
    public enum VT : byte
    {
        Null = 0, String, Number, Object, Array, Bytes, True, False
    }

    /// <summary>
    /// A member represents either a value or a name/value pair, depending on the presence of the name property.
    /// </summary>
    public struct Member : IKeyed
    {
        // type of the value
        readonly VT vt;

        // Obj, Arr, string, byte[]
        readonly object refv;

        readonly Number numv;

        // key as in an object member
        string key;

        internal Member(Obj v)
        {
            vt = VT.Object;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal Member(Arr v)
        {
            vt = VT.Array;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal Member(string v)
        {
            vt = VT.String;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal Member(byte[] v)
        {
            vt = VT.Bytes;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal Member(bool v)
        {
            vt = v ? VT.True : VT.False;
            refv = null;
            numv = default(Number);
            key = null;
        }

        internal Member(Number v)
        {
            vt = VT.Number;
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
            if (v.vt == VT.Object)
            {
                return (Obj)v.refv;
            }
            return null;
        }

        public static implicit operator Arr(Member v)
        {
            if (v.vt == VT.Array)
            {
                return (Arr)v.refv;
            }
            return null;
        }

        public static implicit operator bool(Member v)
        {
            return v.vt == VT.True;
        }

        public static implicit operator short(Member v)
        {
            if (v.vt == VT.Number)
            {
                return v.numv.Int16;
            }
            return 0;
        }

        public static implicit operator int(Member v)
        {
            if (v.vt == VT.Number)
            {
                return v.numv.Int32;
            }
            return 0;
        }

        public static implicit operator long(Member v)
        {
            if (v.vt == VT.Number)
            {
                return v.numv.Int64;
            }
            return 0;
        }

        public static implicit operator decimal(Member v)
        {
            if (v.vt == VT.Number)
            {
                return v.numv.Decimal;
            }
            return 0;
        }

        public static implicit operator DateTime(Member v)
        {
            if (v.vt == VT.String)
            {
                return default(DateTime);
            }
            return default(DateTime);
        }

        public static implicit operator byte[] (Member v)
        {
            if (v.vt == VT.String)
            {
                return (byte[])v.refv;
            }
            return null;
        }

        public static implicit operator string(Member v)
        {
            if (v.vt == VT.String)
            {
                return (string)v.refv;
            }
            return null;
        }
    }
}