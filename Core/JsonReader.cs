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
            JsonReader jc = new JsonReader(b, b.Length);

            string name = null;
            int age = 0;
            jc.ReadObject(() =>
            {
                jc.Read(nameof(name), ref name);
                jc.Read(nameof(age), ref age);
            });

        }
    }

    public class JsonReader : ContentReader, ISerialReader
    {
        JsonKnot[] knots = new JsonKnot[8];

        // current level, start with 0
        int level = -1;

        int pos;


        public JsonReader(byte[] buffer, int count) : base(buffer, count)
        {
        }

        public override string Type => "application/json";


        internal void SkipLevel()
        {

        }

        internal bool SeekInKnot(string name)
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
                    knots[level].end = p; // mark down the level end
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
                    knots[level].end = p; // mark down the level end
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


        public bool Read(ref bool value)
        {
            throw new NotImplementedException();
        }

        internal bool Read(ref decimal value)
        {
            return false;
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

        public bool ReadObject(Action a)
        {
            while (buffer[pos] != '{')
            {
                pos++;
            }
            level++;

            a();

            level--;
            while (buffer[pos] != '}')
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
            ReadArray(() => { o.ReadFrom(this); });
            value = o;
            return true;
        }

        public bool Read(ref short value)
        {
            return false;
        }

        void SkipWs()
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
            if (SeekInKnot(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read(string name, ref int value)
        {
            if (SeekInKnot(name))
            {
                return Read(ref value);
            }
            return false;
        }

        public bool Read(string name, ref decimal value)
        {
            if (SeekInKnot(name))
            {
                return Read(ref value);
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
            SeekInKnot(name);

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

        public bool Read(string name, ref char[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref long value)
        {
            throw new NotImplementedException();
        }

        bool ISerialReader.Read(ref decimal value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref char[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(ref T[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(ref Dictionary<string, T> value)
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
    }
}