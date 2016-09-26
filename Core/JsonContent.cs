using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JsonContent : DynamicContent, ISerialReader, ISerialWriter
    {
        // stack parsing traces
        readonly Trace[] traces;

        // current level in stack
        int level;

        // current position
        int pos;


        public JsonContent(byte[] buf, int count) : base(buf, count)
        {
            traces = new Trace[8];
            level = -1;
            pos = -1;
        }

        public override string Type => "application/json";


        public bool ReadArray(Action a)
        {
            // enter the openning bracket
            int p = pos;
            for (;;)
            {
                p++;
                if (p >= count) return false;
                byte c = buffer[p];
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                {
                    continue;
                }
                if (c == '[')
                {
                    level++;
                    traces[level].IsArray = true;
                    traces[level].Start = p;
                    pos = p;
                    break;
                }
                return false;
            }

            a?.Invoke();

            // exit the closing bracket
            p = pos;
            for (;;)
            {
                byte c = buffer[p++];
                if (p >= count) return false;
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                {
                    continue;
                }
                if (c == ']')
                {
                    level--;
                    pos = p;
                    return true;
                }
                return false;
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

            // find first quotation mark
            int p = pos;
            int start = -1;
            for (;;)
            {
                p++;
                if (p >= count) return false;
                byte c = buffer[p];
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n') continue;
                if (c == '"')
                {
                    start = p;
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
                if (c == '"') // ending quotation mark
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
            ReadArray(() => { o.ReadFrom(this); });
            return true;
        }

        public bool Read(ref byte[] value)
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
                if (p >= count) return false;
                byte c = buffer[p];
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                {
                    continue;
                }
                if (c == '{')
                {
                    level++;
                    traces[level].IsArray = true;
                    traces[level].Start = p;
                    pos = p;
                    break;
                }
                return false;
            }

            a?.Invoke();

            // exit the closing brace
            p = pos;
            for (;;)
            {
                byte c = buffer[p++];
                if (p >= count) return false;
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                {
                    continue;
                }
                if (c == '}')
                {
                    level--;
                    pos = p;
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// If success, reposition to the colon right after the found name.
        /// </summary>
        /// <param name="name">the name string to be found</param>
        /// <returns>true if found</returns>
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
                    traces[level].Ordinal++;
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


        public void WriteArray(Action a)
        {
            level++;
            Put('[');

            traces[level].IsArray = true;
            a();

            Put(']');
            level--;
        }

        public void WriteObject(Action a)
        {
            level++;
            Put('{');

            traces[level].IsArray = false;
            a();

            Put('}');
            level--;
        }


        public void Write(string name, short value)
        {
            if (traces[level].Ordinal > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            traces[level].Ordinal++;
        }

        public void Write(string name, int value)
        {
            if (traces[level].Ordinal > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            traces[level].Ordinal++;
        }

        public void Write(string name, decimal value)
        {
            if (traces[level].Ordinal > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            traces[level].Ordinal++;
        }

        public void Write(string name, DateTime value)
        {
            if (traces[level].Ordinal > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            traces[level].Ordinal++;
        }

        public void Write(string name, string value)
        {
            if (traces[level].Ordinal > 0)
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

            traces[level].Ordinal++;
        }

        public void Write<T>(string name, T value) where T : ISerial
        {
            if (traces[level].Ordinal > 0)
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

            traces[level].Ordinal++;
        }

        public void Write(string name, List<ISerial> list)
        {
            if (traces[level].Ordinal > 0)
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

            traces[level].Ordinal++;
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
                    obj.WriteTo(this);
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