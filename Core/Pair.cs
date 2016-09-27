using System;

namespace Greatbone.Core
{
    public class Pair : Value, IMember
    {
        string name;

        public string Key => name;

    }
}