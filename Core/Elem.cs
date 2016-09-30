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
    /// An element represents either a value or a name/value pair, depending on the presence of the name property.
    /// </summary>
    public struct Elem : IKeyed
    {
        // type of the value
        readonly VT vt;

        // Obj, Arr, string, byte[]
        readonly object refv;

        readonly Number numv;

        // key as in an object member
        string key;

        internal Elem(Obj v)
        {
            vt = VT.Object;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal Elem(Arr v)
        {
            vt = VT.Array;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal Elem(string v)
        {
            vt = VT.String;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal Elem(byte[] v)
        {
            vt = VT.Bytes;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal Elem(bool v)
        {
            vt = v ? VT.True : VT.False;
            refv = null;
            numv = default(Number);
            key = null;
        }

        internal Elem(Number v)
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

        public static implicit operator Obj(Elem v)
        {
            return (Obj)v.refv;
        }

        public static implicit operator Arr(Elem v)
        {
            return (Arr)v.refv;
        }

        public static implicit operator bool(Elem v)
        {
            return v.vt == VT.True;
        }

        public static implicit operator int(Elem v)
        {
            if (v.vt == VT.Number)
            {
                // return v.numv.Int32();
            }
            return 0;
        }

        public static implicit operator string(Elem v)
        {
            if (v.vt == VT.String)
            {
                return (string)v.refv;
            }
            return null;
        }
    }
}