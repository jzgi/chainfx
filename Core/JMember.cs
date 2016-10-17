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
    /// A member represents a value or a name/value pair if with the name.
    /// </summary>
    public struct JMember : IKeyed
    {
        // type of the value
        internal readonly VT vt;

        // Obj, Arr, string, byte[]
        internal readonly object refv;

        internal readonly Number numv;

        // key as in an object member
        string key;

        internal JMember(JObj v)
        {
            vt = VT.Object;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal JMember(JArr v)
        {
            vt = VT.Array;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal JMember(string v)
        {
            vt = VT.String;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal JMember(byte[] v)
        {
            vt = VT.Bytes;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal JMember(bool v)
        {
            vt = v ? VT.True : VT.False;
            refv = null;
            numv = default(Number);
            key = null;
        }

        internal JMember(Number v)
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

        public static implicit operator JObj(JMember v)
        {
            if (v.vt == VT.Object)
            {
                return (JObj)v.refv;
            }
            return null;
        }

        public static implicit operator JArr(JMember v)
        {
            if (v.vt == VT.Array)
            {
                return (JArr)v.refv;
            }
            return null;
        }

        public static implicit operator bool(JMember v)
        {
            return v.vt == VT.True;
        }

        public static implicit operator char(JMember v)
        {
            if (v.vt == VT.String)
            {
                string str = (string)v.refv;
                if (!string.IsNullOrEmpty(str))
                {
                    return str[0];
                }
            }
            return (char)0;
        }

        public static implicit operator short(JMember v)
        {
            if (v.vt == VT.Number)
            {
                return v.numv.Short;
            }
            return 0;
        }

        public static implicit operator int(JMember v)
        {
            if (v.vt == VT.Number)
            {
                return v.numv.Int;
            }
            return 0;
        }

        public static implicit operator long(JMember v)
        {
            if (v.vt == VT.Number)
            {
                return v.numv.Long;
            }
            return 0;
        }

        public static implicit operator decimal(JMember v)
        {
            if (v.vt == VT.Number)
            {
                return v.numv.Decimal;
            }
            return 0;
        }

        public static implicit operator Number(JMember v)
        {
            if (v.vt == VT.Number)
            {
                return v.numv;
            }
            return default(Number);
        }

        public static implicit operator DateTime(JMember v)
        {
            if (v.vt == VT.String)
            {
                return default(DateTime);
            }
            return default(DateTime);
        }

        public static implicit operator char[] (JMember v)
        {
            if (v.vt == VT.String)
            {
                return (char[])v.refv;
            }
            return null;
        }

        public static implicit operator string(JMember v)
        {
            if (v.vt == VT.String)
            {
                return (string)v.refv;
            }
            return null;
        }

        public static implicit operator byte[] (JMember v)
        {
            if (v.vt == VT.String)
            {
                return (byte[])v.refv;
            }
            return null;
        }

    }
}