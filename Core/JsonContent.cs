using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JsonContent : DynamicContent, IOutput
    {
        // starting positions of each level
        readonly int[] starts;

        // current level
        int level;

        // current position
        int pos;


        public JsonContent(int capacity) : base(capacity)
        {
        }


        public override string Type => "application/json";


        public bool Array(Action a)
        {
            level++;
            Put('[');

            starts[level] = pos;
            a();

            Put(']');
            level--;

            return true;
        }

        public void Write(byte[] value)
        {
            throw new NotImplementedException();
        }

        //
        // READ OBJECT
        //

        public bool Object(Action a)
        {
            level++;
            Put('{');

            starts[level] = pos;
            a();

            Put('}');
            level--;

            return true;
        }


        public void Put(string name, short value)
        {
            if (starts[level] > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

        }

        public void Put(string name, int value)
        {
            if (starts[level] > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

        }

        public void Put(string name, decimal value)
        {
            if (starts[level] > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

        }

        public void Put(string name, DateTime value)
        {
            if (starts[level] > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

        }

        public void Put(string name, string value)
        {
            if (starts[level] > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');

            if (value == null)
            {
                Put("null");
            }
            else
            {
                Put('"');
                Put(value);
                Put('"');
            }

        }



        public void Put(string name, bool value)
        {
        }

        public void Put(string name, byte[] value)
        {
            throw new NotImplementedException();
        }

        public void Put<T>(string name, List<T> list)
        {
        }

        public void Put<V>(string name, Dictionary<string, V> dict)
        {
            Put('"');
            Put(name);
            Put('"');
            Put(':');

            Put('{');
            foreach (var pair in dict)
            {
                Put('"');
                Put(pair.Key);
                Put('"');
                Put(':');

                //				PutValue(pair.Value);
            }

            Put('}');
        }

        public void Write(string name, params string[] array)
        {
            throw new System.NotImplementedException();
        }

        public void Write(string name, params IDat[] array)
        {
            Put('"');
            Put(name);
            Put('"');
            Put(':');

            Put('[');
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (i > 0)
                    {
                        Put(',');
                    }

                    IDat obj = array[i];

                    Put('{');
                    obj.To(this);
                    Put('}');
                }
            }
            Put(']');
        }

        public void Put(string name, long value)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, char[] value)
        {
            throw new NotImplementedException();
        }

        public void Write(bool value)
        {
            throw new NotImplementedException();
        }




        public void Arr(Action a)
        {
            throw new NotImplementedException();
        }

        public void Obj(Action a)
        {
            throw new NotImplementedException();
        }

        public new void Put<T>(string name, T value)
        {
            throw new NotImplementedException();
        }
    }
}