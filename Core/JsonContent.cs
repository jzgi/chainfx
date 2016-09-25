using System;
using System.Collections.Generic;
using System.Text;

namespace Greatbone.Core
{
    public class JsonContent : DynamicContent, ISerialReader, ISerialWriter
    {
        // stack of json knots in processing
        readonly JsonLR[] stack;

        // current level in stack
        int level;

        // current position
        int pos;


        public JsonContent(byte[] buf, int count) : base(buf, count)
        {
            stack = new JsonLR[8];
            level = -1;
            pos = -1;
        }

        public override string Type => "application/json";


        public bool ReadArray(Action a)
        {
            // enter the openning brace
            int p = pos;
            for (;;)
            {
                p++;
                if (p >= count)
                {
                    return false;
                }
                byte c = buffer[p];
                if (c == '[')
                {
                    level++;
                    stack[level].IsArray = true;
                    stack[level].Start = p;
                    pos = p;
                    break;
                }
            }

            a?.Invoke();

            // exit the closing brace
            p = pos;
            while (p < count)
            {
                byte c = buffer[p];
                p++;
                if (c == ']')
                {
                    level--;
                    pos = p;
                    return true;
                }
            }
            return false;        }

        public bool Read(ref bool value)
        {
            int p = pos;
            byte c;
            for (;;) // skip white spaces
            {
                c = buffer[p];
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                {
                    p++;
                    if (p >= count) return false;
                }
                else break;
            }

            if (c == 't' && buffer[p + 1] == 'r' && buffer[p + 2] == 'u' && buffer[p + 3] == 'e')
            {
                value = true;
                return true;
            }
            if (c == 'f' && buffer[p + 1] == 'a' && buffer[p + 2] == 'l' && buffer[p + 3] == 's' && buffer[p + 4] == 'e')
            {
                value = false;
                return true;
            }

            return false;
        }

        public bool Read(ref short value)
        {
            value = 0;
            return false;
        }

        public bool Read(ref int value)
        {
            JsonNum num;
            int p = pos;
            byte c;
            for (;;) // skip white spaces
            {
                p++;
                if (p >= count) return false;
                c = buffer[p];
                if (c != ' ' && c != '\t' && c != '\r' && c != '\n') break;
            }

            for (;;)
            {
                if (c >= '0' && c <= '9' || c == '.')
                {
                    num.Add(c);
                    p++;
                }
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
            value = default(DateTime);

            return false;
        }

        public bool Read(ref char[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref string value)
        {
            JsonStr str = new JsonStr(64);
            int p = pos;
            while (++p < count)
            {
                byte c = buffer[p];
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n') continue;
                else if (c == '"')
                {
                    break;
                }
                else
                {
                    return false;
                }
            }


            while (++p < count)
            {
                byte c = buffer[p];
                if (c == '"')
                {
                    pos = p;
                    value = str.ToString();
                    return true;
                }
                else
                {
                    str.Add(c);
                }
            }
            return false;
        }


        public bool Read<T>(ref T value) where T : ISerial, new()
        {
            T o = new T();
            ReadArray(() => { o.ReadFrom(this); });
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
            // enter the openning brace
            int p = pos;
            for (;;)
            {
                p++;
                if (p >= count)
                {
                    return false;
                }
                byte c = buffer[p];
                if (c == '{')
                {
                    level++;
                    stack[level].IsArray = false;
                    stack[level].Start = p;
                    pos = p;
                    break;
                }
            }

            a?.Invoke();

            // exit the closing brace
            p = pos;
            while (p < count)
            {
                byte c = buffer[p];
                p++;
                if (c == '}')
                {
                    level--;
                    pos = p;
                    return true;
                }
            }
            return false;
        }

        internal bool Seek(string name)
        {
            // in between quotation marks
            bool inq = false;

            int p = pos;

            // seek openning quotation
            int qstart = -1;
            for (;;)
            {
                p++;
                if (p >= count) return false;
                byte c = buffer[p];
                if (c == '"')
                {
                    qstart = p;
                    inq = true;
                    break;
                }
            }

            // seek closing quotation
            int qend = -1;
            for (;;)
            {
                p++;
                if (p >= count) return false;
                byte c = buffer[p];
                if (c == '"')
                {
                    qend = p; // mark down second quotation 
                    inq = false;
                    break;
                }
            }

            // locate the colon
            for (;;)
            {
                p++;
                if (p >= count) return false;
                byte c = buffer[p];
                if (c == ':')
                {
                    pos = p;
                    break;
                }
            }

            int k = qstart + 1;
            int len = qend - k;
            bool eq = false;
            if (len == name.Length)
            {
                eq = true;
                for (int i = 0; i < len; i++,k++)
                {
                    if (buffer[k] != name[i])
                    {
                        eq = false;
                        break;
                    }
                }
            }
            return eq;
        }

        public bool Read(string name, ref short value)
        {
            if (Seek(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read(string name, ref int value)
        {
            if (Seek(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read(string name, ref decimal value)
        {
            if (Seek(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read(string name, ref DateTime value)
        {
            if (Seek(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read(string name, ref string value)
        {
            if (Seek(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read<T>(string name, ref T value) where T : ISerial, new()
        {
            if (Seek(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read<T>(string name, List<T> value) where T : ISerial, new()
        {
            Seek(name);

            ReadArray(delegate
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


        public JsonContent(int capacity) : base(capacity)
        {
        }


        public void WriteArray(Action a)
        {
            level++;
            Put('[');

            stack[level].IsArray = true;
            a();

            Put(']');
            level--;
        }

        public void WriteObject(Action a)
        {
            level++;
            Put('{');

            stack[level].IsArray = false;
            a();

            Put('}');
            level--;
        }


        public void Write(string name, short value)
        {
            if (stack[level].Ordinal > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            stack[level].Ordinal++;
        }

        public void Write(string name, int value)
        {
            if (stack[level].Ordinal > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            stack[level].Ordinal++;
        }

        public void Write(string name, decimal value)
        {
            if (stack[level].Ordinal > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            stack[level].Ordinal++;
        }

        public void Write(string name, DateTime value)
        {
            if (stack[level].Ordinal > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            stack[level].Ordinal++;
        }

        public void Write(string name, string value)
        {
            if (stack[level].Ordinal > 0)
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

            stack[level].Ordinal++;
        }

        public void Write<T>(string name, T value) where T : ISerial
        {
            if (stack[level].Ordinal > 0)
            {
                Put(',');
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

            stack[level].Ordinal++;
        }

        public void Write(string name, List<ISerial> list)
        {
            if (stack[level].Ordinal > 0)
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

            stack[level].Ordinal++;
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