using System;

namespace Greatbone.Core
{
    public struct Pair : IMember
    {
        string name;

        Value value;

        public string Key => name;

        public static implicit operator int(Pair v)
        {
            return 0;
        }
        public static implicit operator string(Pair v)
        {
            return v.value.strv;
        }
    }
}