using System.Text;

namespace Greatbone.Core
{

    public class JsonParser
    {
        byte[] array;

        int limit;

        // UTF-8 string builder
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
                byte b = array[++p];
                if (p >= limit) return null;
                if (b == '"')
                {
                    break;
                }
            }

            int close = -1;
            for (;;)
            {
                byte b = array[++p];
                if (p >= limit) return null;
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
                byte b = array[++p];
                if (p >= limit) return null;
                if (b == ':')
                {
                    break;
                }
            }

            //

            byte c = array[p];
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
            else if (c == '"')
            {
                string v = ParseString(p);
                obj.Add(sb.ToString(), v);
            }


            Parse(obj);

            return obj;
        }

        public Arr ParseArr(int left)
        {
            return null;
        }

        public string ParseString(int start)
        {
            return null;
        }
    }
}