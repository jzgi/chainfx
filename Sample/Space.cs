using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Space : IZone, IDump
    {
        internal string key;

        internal string name;

        public char[] Credential { get; set; }

        public long ModifiedOn { get; set; }

        public void To(IOutput o)
        {
            o.Put(nameof(key), key);
            o.Put(nameof(name), name);
        }

        public void From(IInput i)
        {
            i.Got(nameof(key), out key);
            i.Got(nameof(name), out name);
        }

        public string Key => key;


        public struct Address : IDump
        {
            internal string address;
            internal string postal;

            public void From(IInput i)
            {
                i.Got(nameof(address), out address);
                i.Got(nameof(postal), out postal);
            }

            public void To(IOutput o)
            {
            }
        }
    }
}