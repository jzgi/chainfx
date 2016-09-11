using System;
using System.Collections.Generic;
using System.Text;

namespace Greatbone.Core
{

    public class JsonWriter : ContentWriter, ISerialWriter
    {
        JsonKnot[] knots = new JsonKnot[8];

        // current level, start with 0
        int level = -1;

        int pos;


        public override string Type => "application/json";

        public JsonWriter(int capacity) : base(capacity) { }

        internal void SkipLevel()
        {

        }



        //
        // WRITES
        //

        public void WriteArray(Action inner)
        {
            level++;
            Put('[');

            knots[level].array = true;

            Put(']');
            level--;
        }

        public void WriteObject(Action inner)
        {
            Put('{');

            level++;
            knots[level].array = false;

            Put('}');

            level--;
        }


        public void Write(string name, short value)
        {
            if (knots[level].ordinal > 0)
            {
                Put(','); // precede a comma
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            knots[level].ordinal++;
        }

        public void Write(string name, int value)
        {
            if (knots[level].ordinal > 0)
            {
                Put(','); // precede a comma
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            knots[level].ordinal++;
        }

        public void Write(string name, decimal value)
        {
            if (knots[level].ordinal > 0)
            {
                Put(','); // precede a comma
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            knots[level].ordinal++;
        }

        public void Write(string name, DateTime value)
        {
            if (knots[level].ordinal > 0)
            {
                Put(','); // precede a comma
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            knots[level].ordinal++;
        }

        public void Write(string name, string value)
        {
            if (knots[level].ordinal > 0)
            {
                Put(','); // precede a comma
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

            knots[level].ordinal++;
        }

        public void Write<T>(string name, T value) where T : ISerial
        {
            if (knots[level].ordinal > 0)
            {
                Put(','); // precede a comma
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');

            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                Put("null");
            }
            else
            {
                WriteObject(() => { value.WriteTo(this); });
            }

            knots[level].ordinal++;
        }

        public void Write(string name, List<ISerial> list)
        {
            if (knots[level].ordinal > 0)
            {
                Put(','); // precede a comma
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');

            Put('[');
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (i > 0)
                    {
                        Put(',');
                    }

                    ISerial obj = list[i];

                    Put('{');
                    obj.WriteTo(this);
                    Put('}');
                }
            }
            Put(']');

            knots[level].ordinal++;
        }

        public void Write(string name, bool value)
        {
        }

        public void Write<T>(string name, List<T> list)
        {
        }

        public void Write<V>(string name, Dictionary<string, V> dict)
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

        public void Write(string name, params ISerial[] array)
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

                    ISerial obj = array[i];

                    Put('{');
                    obj.WriteTo(this);
                    Put('}');
                }
            }
            Put(']');
        }

        public bool Read(string name, char[] value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, long value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, char[] value)
        {
            throw new NotImplementedException();
        }

        bool ISerialWriter.WriteArray(Action a)
        {
            throw new NotImplementedException();
        }

        bool ISerialWriter.WriteObject(Action a)
        {
            throw new NotImplementedException();
        }

        public bool Write(bool value)
        {
            throw new NotImplementedException();
        }

        public bool Write(short value)
        {
            throw new NotImplementedException();
        }

        public bool Write(int value)
        {
            throw new NotImplementedException();
        }

        public bool Write(long value)
        {
            throw new NotImplementedException();
        }

        public bool Write(decimal value)
        {
            throw new NotImplementedException();
        }

        public bool Write(DateTime value)
        {
            throw new NotImplementedException();
        }

        public bool Write(char[] value)
        {
            throw new NotImplementedException();
        }

        public bool Write(string value)
        {
            throw new NotImplementedException();
        }

        public bool Write<T>(T value) where T : ISerial
        {
            throw new NotImplementedException();
        }

        public bool Write<T>(T[] value)
        {
            throw new NotImplementedException();
        }

        public bool Write<T>(List<T> value)
        {
            throw new NotImplementedException();
        }

        public bool Write<T>(Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }


        public bool Write<T>(string name, T[] value)
        {
            throw new NotImplementedException();
        }

        bool ISerialWriter.Write(string name, bool value)
        {
            throw new NotImplementedException();
        }

        bool ISerialWriter.Write(string name, short value)
        {
            throw new NotImplementedException();
        }

        bool ISerialWriter.Write(string name, int value)
        {
            throw new NotImplementedException();
        }

        bool ISerialWriter.Write(string name, long value)
        {
            throw new NotImplementedException();
        }

        bool ISerialWriter.Write(string name, decimal value)
        {
            throw new NotImplementedException();
        }

        bool ISerialWriter.Write(string name, DateTime value)
        {
            throw new NotImplementedException();
        }

        bool ISerialWriter.Write(string name, char[] value)
        {
            throw new NotImplementedException();
        }

        bool ISerialWriter.Write(string name, string value)
        {
            throw new NotImplementedException();
        }

        bool ISerialWriter.Write<T>(string name, T value)
        {
            throw new NotImplementedException();
        }

        bool ISerialWriter.Write<T>(string name, List<T> value)
        {
            throw new NotImplementedException();
        }

        bool ISerialWriter.Write<T>(string name, Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }
    }
}