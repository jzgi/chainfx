using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Space : IZone, IData
    {
        internal string key;

        internal string name;

        public char[] Credential { get; set; }

        public long ModifiedOn { get; set; }

        public void To(IDataOutput o, int flags)
        {
            o.Put(nameof(key), key);
            o.Put(nameof(name), name);
        }

        public void From(IDataInput i, int flags)
        {
            i.Got(nameof(key), out key);
            i.Got(nameof(name), out name);
        }

        public string Key => key;


        public struct Address : IData
        {
            internal string address;
            internal string postal;

            public void From(IDataInput i, int flags)
            {
                i.Got(nameof(address), out address);
                i.Got(nameof(postal), out postal);
            }

            public void To(IDataOutput o, int flags)
            {
            }
        }
    }
}