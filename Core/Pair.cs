using System;

namespace Greatbone.Core
{
    public struct Pair : IKeyed
    {
        string key;

        // string or string array
        object values;

        int count;

        public string Key => key;

        internal Pair(string key, string v)
        {
            this.key = key;
            values = v;
            count = 1;
        }

        internal void Add(string v)
        {
            if (count == 1) // a single string
            {
                string old = (string)values;
                string[] arr = new string[8];
                arr[0] = old;
                arr[1] = v;
                count = 2;
            }
            else
            {
                // ensure capacity
                string[] arr = (string[])values;
                int len = arr.Length;
                if (count >= len)
                {
                    string[] alloc = new string[len * 4];
                    Array.Copy(arr, 0, alloc, 0, len);
                    values = arr = alloc;
                }
                arr[count++] = v;
            }
        }

        public string String
        {
            get
            {
                if (count == 1) return (string)values;
                return ((string[])values)[0];
            }
        }

        public string[] Strings
        {
            get
            {
                if (count == 1) return new string[] { (string)values };
                return (string[])values;
            }
        }

        public int Int
        {
            get
            {
                if (count == 1)
                {
                    string str = (string)values;
                    int v;
                    if (int.TryParse(str, out v))
                    {
                        return v;
                    }
                }
                return 0;
            }
        }

        public int[] Ints
        {
            get
            {
                if (count == 1)
                {
                    string str = (string)values;
                    int v;
                    if (int.TryParse(str, out v))
                    {
                        return new int[] { v };
                    }
                }
                return null;
            }
        }

        //
        // CONVERSION
        //

        public static implicit operator bool(Pair v)
        {
            return false;
        }

        public static implicit operator short(Pair v)
        {
            return 0;
        }

        public static implicit operator int(Pair v)
        {
            return v.Int;
        }

        public static implicit operator long(Pair v)
        {
            return 0;
        }

        public static implicit operator decimal(Pair v)
        {
            return 0;
        }

        public static implicit operator Number(Pair v)
        {
            return default(Number);
        }

        public static implicit operator DateTime(Pair v)
        {
            return default(DateTime);
        }

        public static implicit operator char[] (Pair v)
        {
            return null;
        }

        public static implicit operator string(Pair v)
        {
            return null;
        }

        public static implicit operator byte[] (Pair v)
        {
            return null;
        }

    }

}