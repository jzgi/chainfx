using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JsonContent : DynamicContent, ISerialReader, ISerialWriter
    {
        // starting positions of each level
        readonly int[] starts;

        // current level
        int level;

        // current position
        int pos;

        readonly bool reading;


        public JsonContent(byte[] buf, int count) : base(buf, count)
        {
            starts = new int[8];
            level = -1;
            pos = -1;
            reading = true;
        }

        public override string Type => "application/json";


        public bool Array(Action a)
        {
            if (reading)
            {
                // enter the start bracket
                int p = pos;
                for (;;)
                {
                    if (++p >= count) return false;
                    byte c = buffer[p];
                    if (c == ' ' || c == '\t' || c == '\r' || c == '\n') continue;
                    if (c == '[')
                    {
                        // markdown the start position
                        starts[++level] = p;
                        pos = p;
                        break;
                    }
                    return false;
                }

                a?.Invoke();

                // exit the end bracket
                p = pos;
                for (;;)
                {
                    byte c = buffer[p++];
                    if (p >= count) return false;
                    if (c == ']')
                    {
                        level--;
                        pos = ++p; // skip out
                        return true;
                    }
                    else
                    {
                        p++;
                        continue;
                    }
                }
            }
            else // not reading
            {
                level++;
                Put('[');

                starts[level] = pos;
                a();

                Put(']');
                level--;

                return true;
            }
        }

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
            return false;
        }

        public void Write(byte[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref int value)
        {
            Number num = new Number();
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
            return false;
        }

        public bool Read(ref char[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref string value)
        {
            Str str = new Str(64);

            // find opening quotation
            int p = pos;
            int open = -1;
            for (;;)
            {
                p++;
                if (p >= count) return false;
                byte c = buffer[p];
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n') continue;
                if (c == '"')
                {
                    open = p;
                    break;
                }
                return false;
            }

            // adding string content
            for (;;)
            {
                p++;
                if (p >= count) return false;
                byte c = buffer[p];
                if (c == '"') // closing quotation
                {
                    pos = p;
                    value = str.ToString();
                    return true;
                }
                str.Add(c);
            }
        }


        public bool Read<T>(ref T value) where T : ISerial, new()
        {
            T o = new T();
            Array(() => { o.From(this); });
            return true;
        }

        public bool Read(ref byte[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(ref List<T> list)
        {
            List<T> lst = new List<T>();
            Array(() =>
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

        public bool Object(Action a)
        {
            if (reading)
            {
                // enter the start bracket
                int p = pos;
                for (;;)
                {
                    if (++p >= count) return false;
                    byte c = buffer[p];
                    if (c == ' ' || c == '\t' || c == '\r' || c == '\n') continue;
                    if (c == '{')
                    {
                        // markdown the start position
                        starts[++level] = p;
                        pos = p;
                        break;
                    }
                    return false;
                }

                a?.Invoke();

                // exit the end bracket
                p = pos;
                for (;;)
                {
                    byte c = buffer[p++];
                    if (p >= count) return false;
                    if (c == '}')
                    {
                        level--;
                        pos = ++p; // skip out
                        return true;
                    }
                    else
                    {
                        p++;
                        continue;
                    }
                }
            }
            else // not reading mode
            {
                level++;
                Put('{');

                starts[level] = pos;
                a();

                Put('}');
                level--;

                return true;
            }
        }

        /// <summary>
        /// If success, reposition to the colon right after the found name.
        /// </summary>
        /// <param name="name">the name string to be found</param>
        /// <returns>true if found</returns>
        internal bool Seek(string name)
        {
            int p = pos;
            // seek openning quotation
            int open = -1;
            for (;;)
            {
                if (++p >= count) return false;
                byte c = buffer[p];
                if (c == '"')
                {
                    open = p;
                    break;
                }
            }

            // seek closing quotation
            int close = -1;
            for (;;)
            {
                if (++p >= count) return false;
                byte c = buffer[p];
                if (c == '"')
                {
                    close = p;
                    break;
                }
            }

            // seek the colon
            for (;;)
            {
                if (++p >= count) return false;
                byte c = buffer[p];
                if (c == ':')
                {
                    pos = p; // reposition to colon
                    break;
                }
            }

            int j = open + 1;
            int len = close - j;
            bool eq = false;
            if (len == name.Length)
            {
                eq = true;
                for (int i = 0; i < len; i++, j++)
                {
                    if (buffer[j] != name[i])
                    {
                        eq = false;
                        break;
                    }
                }
            }
            return eq;
        }

        public bool Read(string name, ref bool value)
        {
            if (Seek(name))
            {
                return Read(ref value);
            }
            return false;
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

        public bool Read(string name, ref long value)
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

        public bool Read(string name, ref byte[] value)
        {
            if (Seek(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read<T>(string name, List<T> value) where T : ISerial, new()
        {
            if (Seek(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read(string name, ref char[] value)
        {
            if (Seek(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read<T>(string name, ref List<T> value)
        {
            if (Seek(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read<T>(string name, ref Dictionary<string, T> value)
        {
            if (Seek(name))
            {
                return Read(ref value);
            }
            return false;
        }


        //
        // WRITES
        //


        public JsonContent(int capacity) : base(capacity)
        {
        }





        public void Write(string name, short value)
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

        public void Write(string name, int value)
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

        public void Write(string name, decimal value)
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

        public void Write(string name, DateTime value)
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

        public void Write(string name, string value)
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

        public void Write<T>(string name, T value) where T : ISerial
        {
            if (starts[level] > 0)
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
                Object(() => { value.To(this); });
            }

        }

        public void Write(string name, List<ISerial> list)
        {
            if (starts[level] > 0)
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
                    obj.To(this);
                    Put('}');
                }
            }
            Put(']');

        }

        public void Write(string name, bool value)
        {
        }

        public void Write(string name, byte[] value)
        {
            throw new NotImplementedException();
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
                    obj.To(this);
                    Put('}');
                }
            }
            Put(']');
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

        public void Write<T>(List<T> value)
        {
            throw new NotImplementedException();
        }

        public void Write<T>(Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }
    }
}