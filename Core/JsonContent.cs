using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JsonContent : DynamicContent, ISerialReader, ISerialWriter
    {
        Knob[] stack = new Knob[8];

        int level = -1;

        int pos;


        public JsonContent(byte[] buffer) : base(buffer)
        {
        }

        public JsonContent(byte[] buffer, int count) : base(buffer, count)
        {
        }

        public override string Type => "application/json";

        internal void SkipTill(byte c)
        {
        }

        internal void SkipWsTill(byte c)
        {
        }

        internal bool LocateNameAtLevel(string name)
        {
            int p = stack[level].current;

            // seek two quotations and a colon
            while (buffer[p] != ':')
            {
                p++;
            }


            return false;
        }

        internal bool GetValue(ref short value)
        {
            return false;
        }

        internal bool GetValue(ref int value)
        {
            return false;
        }


        internal bool GetValue(ref decimal value)
        {
            return false;
        }

        public bool Read<T>(string name, ref List<T> value) where T : ISerial, new()
        {
            throw new NotImplementedException();
        }

        public bool ReadObject(Action a)
        {
            while (buffer[pos] != '[')
            {
                pos++;
            }
            level++;

            a();

            level--;
            while (buffer[pos] != ']')
            {
                pos++;
            }
            return true;
        }

        public bool ReadArray(Action a)
        {
            while (buffer[pos] != '[')
            {
                pos++;
            }
            level++;

            a();

            level--;
            while (buffer[pos] != ']')
            {
                pos++;
            }
            return true;
        }

        public bool Read<T>(ref List<T> list)
        {
            List<T> lst = new List<T>();
            ReadArray(() =>
            {
                if (typeof(T) == typeof(int))
                {
                    string value = null;
                    if (Read(ref value))
                    {
                    }
                }
            });
            return false;
        }

        public bool Read<T>(ref T value) where T : ISerial, new()
        {
            T o = (value != null) ? value : new T();
            ReadObject(() => { o.ReadFrom(this); });
            value = o;
            return true;
        }

        public bool Read(ref short value)
        {
            return false;
        }

        public bool Read(ref int value)
        {
            return false;
        }

        public bool Read(ref DateTime value)
        {
            return false;
        }

        public bool Read(ref string value)
        {
            return false;
        }


        public bool Read(string name, ref short value)
        {
            if (LocateNameAtLevel(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read(string name, ref int value)
        {
            if (LocateNameAtLevel(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read(string name, ref decimal value)
        {
            if (LocateNameAtLevel(name))
            {
                return GetValue(ref value);
            }
            return false;
        }

        public bool Read(string name, ref DateTime value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref string value)
        {
            throw new System.NotImplementedException();
        }

        public bool Read<T>(string name, ref T value) where T : ISerial, new()
        {
            throw new System.NotImplementedException();
        }

        public bool Read<T>(string name, List<T> value) where T : ISerial, new()
        {
            LocateNameAtLevel(name);

            ReadArray(() =>
            {
                T o = new T();
                while (Read(ref o))
                {
                    value.Add(o);
                }
            });
            return false;
        }

        public bool Read(string name, ref List<string> value)
        {
            throw new System.NotImplementedException();
        }

        public bool Read(string name, ref bool value)
        {
            throw new System.NotImplementedException();
        }

        public bool Read<K, V>(string name, ref Dictionary<K, V> value)
        {
            throw new System.NotImplementedException();
        }

        public bool Read(string name, ref string[] value)
        {
            throw new System.NotImplementedException();
        }


        //
        // WRITES
        //

        public void WriteArray(Action inner)
        {
            level++;
            Put('[');

            stack[level].array = true;

            Put(']');
            level--;
        }

        public void WriteObject(Action inner)
        {
            Put('{');

            level++;
            stack[level].array = false;

            Put('}');

            level--;
        }


        public void Write(string name, short value)
        {
            if (stack[level].ordinal > 0)
            {
                Put(','); // precede a comma
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            stack[level].ordinal++;
        }

        public void Write(string name, int value)
        {
            if (stack[level].ordinal > 0)
            {
                Put(','); // precede a comma
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            stack[level].ordinal++;
        }

        public void Write(string name, decimal value)
        {
            if (stack[level].ordinal > 0)
            {
                Put(','); // precede a comma
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            stack[level].ordinal++;
        }

        public void Write(string name, DateTime value)
        {
            if (stack[level].ordinal > 0)
            {
                Put(','); // precede a comma
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            stack[level].ordinal++;
        }

        public void Write(string name, string value)
        {
            if (stack[level].ordinal > 0)
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

            stack[level].ordinal++;
        }

        public void Write<T>(string name, T value) where T : ISerial
        {
            if (stack[level].ordinal > 0)
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

            stack[level].ordinal++;
        }

        public void Write(string name, List<ISerial> list)
        {
            if (stack[level].ordinal > 0)
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

            stack[level].ordinal++;
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
    }
}