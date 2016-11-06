using System;

namespace Greatbone.Core
{
    public struct Attr : IKeyed
    {
        readonly string key;

        string value;

        public string Key => key;

        internal Attr(string key, string v)
        {
            this.key = key;
            value = v;
        }



    }

}