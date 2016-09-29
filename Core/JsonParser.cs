using System.Text;

namespace Greatbone.Core
{

    public class JsonParser
    {
        byte[] buffer;

        int count;

        Str str;

        public void Parse(Obj obj)
        {

        }

        public Obj ParseObj(int start)
        {
            Obj obj = new Obj();

            int p = start;

            StringBuilder sb = new StringBuilder();
            int lq = 0;
            for (;;)
            {
                byte b = buffer[++p];
                if (p >= count) return null;
                if (b == '"')
                {
                    break;
                }
            }

            int close = -1;
            for (;;)
            {
                byte b = buffer[++p];
                if (p >= count) return null;
                if (b == '"')
                {
                    break;
                }
                else
                {
                    sb.Append((char)b);
                }
            }

            for (;;)
            {
                byte b = buffer[++p];
                if (p >= count) return null;
                if (b == ':')
                {
                    break;
                }
            }

            //

            byte c = buffer[p];
            if (c == '{')
            {
                Obj v = ParseObj(p);
                obj.Add(sb.ToString(), v);
            }
            else if (c == '[')
            {
                Obj v = ParseObj(p);
                obj.Add(sb.ToString(), v);
            }


            Parse(obj);

            return obj;
        }
        public Arr ParseArr(int left)
        {
            return null;
        }
    }
}