using System;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// Represents a value or a name/value pair if with the name.
    /// </summary>
    ///
    public struct JMember : IKeyed
    {

        // type of the value
        internal readonly JType type;

        // Obj, Arr, string, byte[]
        internal readonly object refv;

        internal readonly Number numv;

        // key as in an object member
        string key;

        internal JMember(JObj v)
        {
            type = JType.Object;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal JMember(JArr v)
        {
            type = JType.Array;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal JMember(string v)
        {
            type = JType.String;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal JMember(byte[] v)
        {
            type = JType.Bytes;
            refv = v;
            numv = default(Number);
            key = null;
        }

        internal JMember(bool v)
        {
            type = v ? JType.True : JType.False;
            refv = null;
            numv = default(Number);
            key = null;
        }

        internal JMember(Number v)
        {
            type = JType.Number;
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
            if (v.type == JType.Object)
            {
                return (JObj)v.refv;
            }
            return null;
        }

        public static implicit operator JArr(JMember v)
        {
            if (v.type == JType.Array)
            {
                return (JArr)v.refv;
            }
            return null;
        }

        public static implicit operator bool(JMember v)
        {
            return v.type == JType.True;
        }

        public static implicit operator short(JMember v)
        {
            if (v.type == JType.Number)
            {
                return v.numv.Short;
            }
            return 0;
        }

        public static implicit operator int(JMember v)
        {
            if (v.type == JType.Number)
            {
                return v.numv.Int;
            }
            return 0;
        }

        public static implicit operator long(JMember v)
        {
            if (v.type == JType.Number)
            {
                return v.numv.Long;
            }
            return 0;
        }

        public static implicit operator decimal(JMember v)
        {
            if (v.type == JType.Number)
            {
                return v.numv.Decimal;
            }
            return 0;
        }

        public static implicit operator Number(JMember v)
        {
            if (v.type == JType.Number)
            {
                return v.numv;
            }
            return default(Number);
        }

        public static implicit operator DateTime(JMember v)
        {
            if (v.type == JType.String)
            {
                string str = (string)v.refv;
                DateTime dt;
                if (StrUtil.TryParseDate(str, out dt)) return dt;
            }
            return default(DateTime);
        }

        public static implicit operator char[] (JMember v)
        {
            if (v.type == JType.String)
            {
                string str = (string)v.refv;
                return str.ToCharArray();
            }
            return null;
        }

        public static implicit operator string(JMember v)
        {
            if (v.type == JType.String)
            {
                return (string)v.refv;
            }
            return null;
        }

        public static implicit operator byte[] (JMember v)
        {
            if (v.type == JType.String)
            {
                return (byte[])v.refv;
            }
            return null;
        }

    }
}