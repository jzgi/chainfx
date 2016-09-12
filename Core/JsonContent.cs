using System;
using System.Collections.Generic;
using System.Text;

namespace Greatbone.Core
{

    public class JsonContentTest
    {
        public static void Test()
        {
            string rec = "{\"name\":\"jay\", \"age\":12}";

            byte[] b = Encoding.UTF8.GetBytes(rec);
            JsonContent jc = new JsonContent(b, b.Length);

            string name = null;
            int age = 0;
            jc.ReadObject(() =>
            {
                jc.Read(nameof(name), ref name);
                jc.Read(nameof(age), ref age);
            });

        }
    }

    public class JsonContent : DynamicContent, ISerialReader, ISerialWriter
    {
        // stack of json knots in processing
        readonly JsonSeg[] stack;

        // current level in stack
        int level;

        // current position
        int pos;


        public JsonContent(byte[] buffer, int count) : base(buffer, count)
        {
            stack = new JsonSeg[8];
            level = -1;
            pos = -1;
        }

        public override string Type => "application/json";


        public bool ReadArray(Action a)
        {
            int start;
            int p = pos;
            while (p++ < count)
            {
                byte c = buffer[p];
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n') continue; // skip whitespace
                else if (c == '[')
                {
                    start = p;
                    pos = p;
                    break;
                }
                else
                {
                    return false;
                }
            }

            a();

            // skip till a closing brace
            p = pos;
            while (p++ < count)
            {
                byte c = buffer[p];
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n') continue; // skip whitespace
                else if (c == ']')
                {
                    start = p;
                    pos = p;
                    return true;
                }
            }
            return false;
        }

        internal void SkipWs()
        {
            for (;;)
            {
                byte c = buffer[pos];
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                {
                    pos++;
                    continue;
                }
                return;
            }
        }

        public bool Read(ref bool value)
        {
            int p = pos;

            SkipWs();

            if (buffer[p] == 't' && buffer[p + 1] == 'r' && buffer[p + 2] == 'u' && buffer[p + 3] == 'e')
            {
                value = true;
                return true;
            }
            else if (buffer[p] == 'f' && buffer[p + 1] == 'a' && buffer[p + 2] == 'l' && buffer[p + 3] == 's' && buffer[p + 4] == 'e')
            {
                value = false;
                return true;
            }

            SkipWs();

            return false;
        }

        public bool Read(ref short value)
        {
            return false;
        }

        public bool Read(ref int value)
        {
            SkipWs();

            for (;;)
            {
                byte c = buffer[pos];
                if (c != ',' && c == '}')
                {
                }
                break;
            }

            return false;
        }

        public bool Read(ref long value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref decimal value)
        {
            return false;
        }

        public bool Read(ref DateTime value)
        {
            return false;
        }

        public bool Read(ref char[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref string value)
        {
            return false;
        }


        public bool Read<T>(ref T value) where T : ISerial, new()
        {
            T o = (value != null) ? value : new T();
            ReadArray(() => { o.ReadFrom(this); });
            value = o;
            return true;
        }

        public bool Read<T>(ref T[] value)
        {
            throw new NotImplementedException();
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

        public bool Read<T>(ref Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }

        char[] seg = new char[1024];
        int segc = 0;

        void Token()
        {
            for (;;)
            {
                byte c = buffer[pos];
                if (c == ' ' || c == '\t')
                {
                    pos++;
                    continue;
                }
                break;
            }
        }

        //
        // READ OBJECT
        //

        public bool ReadObject(Action a)
        {
            int p = pos;
            while (p++ < count)
            {
                byte c = buffer[p];
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n') continue; // skip whitespace
                else if (c == '{')
                {
                    stack[level].start = p;
                    pos = p;
                    break;
                }
                else
                {
                    return false;
                }
            }

            a();

            // skip till a closing brace
            p = pos;
            while (p++ < count)
            {
                byte c = buffer[p];
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n') continue; // skip whitespace
                else if (c == '}')
                {
                    pos = p;
                    return true;
                }
            }
            return false;
        }

        internal bool Locate(string name)
        {
            // in between quotation marks 
            bool inq;

            int qstart, qend;
            int p = pos;
            for (;;) // find the start quotation
            {
                byte c = buffer[p];
                if (c == '"')
                {
                    inq = true;
                    qstart = p;
                }
                else if (c == '}')
                {
                    stack[level].end = p; // mark down the level end
                }
                p++;
                break;
            }

            for (;;)
            {
                byte c = buffer[p];
                if (c != '"')
                {
                    p++;
                }
                if (c == '}')
                {
                    stack[level].end = p; // mark down the level end
                }
                break;
            }

            SkipWs();

            // seek two quotations and a colon
            while (buffer[pos] != ':')
            {
                pos++;
            }


            return false;
        }

        public bool Read(string name, ref short value)
        {
            if (Locate(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read(string name, ref int value)
        {
            if (Locate(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read(string name, ref decimal value)
        {
            if (Locate(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read(string name, ref DateTime value)
        {
            if (Locate(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read(string name, ref string value)
        {
            if (Locate(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read<T>(string name, ref T value) where T : ISerial, new()
        {
            if (Locate(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read<T>(string name, List<T> value) where T : ISerial, new()
        {
            Locate(name);

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

        public bool Read(string name, ref bool value)
        {
            throw new System.NotImplementedException();
        }

        public bool Read(string name, ref char[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref long value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(string name, ref T[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(string name, ref List<T> value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(string name, ref Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }


        //
        // WRITES
        //


        public JsonContent(int capacity) : base(capacity) { }


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

        public void Write(bool value)
        {
            throw new NotImplementedException();
        }

        public void Write(short value)
        {
            throw new NotImplementedException();
        }

        public void Write(int value)
        {
            throw new NotImplementedException();
        }

        public void Write(long value)
        {
            throw new NotImplementedException();
        }

        public void Write(decimal value)
        {
            throw new NotImplementedException();
        }

        public void Write(DateTime value)
        {
            throw new NotImplementedException();
        }

        public void Write(char[] value)
        {
            throw new NotImplementedException();
        }

        public void Write(string value)
        {
            throw new NotImplementedException();
        }

        public void Write<T>(T value) where T : ISerial
        {
            throw new NotImplementedException();
        }

        public void Write<T>(T[] value)
        {
            throw new NotImplementedException();
        }

        public void Write<T>(List<T> value)
        {
            throw new NotImplementedException();
        }

        public void Write<T>(Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }

        public void Write<T>(string name, T[] value)
        {
            throw new NotImplementedException();
        }
    }
}