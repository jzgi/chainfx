using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JsonText : ISerialReader, ISerialWriter
    {
        // for input
        private string text;

        // for output
        private char[] buffer;

        private int pos;

        // parsing context for levels

        JsonKnot[] levels = new JsonKnot[8];

        int level;

        public JsonText(int capacity)
        {
        }

        public JsonText(string str)
        {
            this.text = str;
        }

        public bool Read(ref bool value)
        {
            throw new NotImplementedException();
        }

        public bool ReadLeft()
        {
            level++; // increase nesting
            int p = levels[level].pos;
            while (++p < text.Length)
            {
                char c = text[p];
                if (c == '{')
                {
                    levels[level].start = p;
                    return true;
                }
            }
            return false;
        }

        public bool Read(ref int value)
        {
            throw new NotImplementedException();
        }

        public bool ReadRight()
        {
            int p = levels[level].pos;
            while (++p < text.Length)
            {
                char c = text[p];
                if (c == '}')
                {
                    levels[level].end = p;
                    level++; // decrease nesting
                    return true;
                }
            }
            return false;
        }

        public bool ReadName(string name)
        {
            int p = levels[level].pos;
            while (++p < text.Length)
            {
                char c = text[p];
                if (c == '"')
                {
                    while (++p > text.Length && text[p] != '"') ;
                    return true;
                }
            }
            return false;
        }

        public bool ReadArray(Action a)
        {
            throw new NotImplementedException();
        }

        public bool ReadObject(Action a)
        {
            throw new NotImplementedException();
        }

        bool ISerialReader.Read<T>(string name, ref List<T> value)
        {
            return Read(name, ref value);
        }

        public void Write<T>(string name, T value) where T : ISerial
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref int value)
        {
            if (ReadName(name))
            {
                int p = levels[level].pos;
            }
            return false;
        }

        public bool Read(string name, ref bool value)
        {
            if (ReadName(name))
            {
                int p = levels[level].pos;
            }
            return false;
        }

        public bool Read(string name, ref short value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref DateTime value)
        {
            throw new NotImplementedException();
        }

        public bool Read<K, V>(string name, ref Dictionary<K, V> value)
        {
            throw new System.NotImplementedException();
        }

        public bool Read(string name, ref decimal value)
        {
            if (ReadName(name))
            {
                int p = levels[level].pos;
            }
            return false;
        }

        public bool Read(string name, ref string value)
        {
            if (ReadName(name))
            {
                int p = levels[level].pos;
            }
            return false;
        }

        public bool Read<T>(string name, ref T value) where T : ISerial, new()
        {
            if (ReadName(name))
            {
                int p = levels[level].pos;
            }
            return false;
        }

        public bool Read(string name, ref List<string> value)
        {
            if (ReadName(name))
            {
                int p = levels[level].pos;
            }
            return false;
        }

        public bool Read(string name, ref string[] value)
        {
            if (ReadName(name))
            {
                int p = levels[level].pos;
            }
            return false;
        }

        public bool Read<T>(string name, ref List<T> value) where T : ISerial
        {
            if (ReadName(name))
            {
                int p = levels[level].pos;
            }
            return false;
        }

        //
        // WRITES
        //


        public void WriteStart(bool array)
        {
            throw new NotImplementedException();
        }

        public void WriteEnd(bool array)
        {
            throw new NotImplementedException();
        }

        void WriteName(string name)
        {
        }

        public void Write(string name, int value)
        {
            WriteName(name);
            //			Put(':);
            //			Put(value);
        }

        public void Write(string name, decimal value)
        {
            WriteName(name);
            //			Put(':);
            //			Put(value);
        }

        public void Write(string name, string value)
        {
            WriteName(name);
            //			Put(':);
            //			Put(value);
        }

        public void Write(string name, short value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, DateTime value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, ISerial value)
        {
            WriteName(name);
            //			Put(':);
            //			Put(value);
        }

        public void Write(string name, bool value)
        {
            WriteName(name);
            //			Put(':);
            //			Put(value);
        }

        public void Write<T>(string name, List<T> list)
        {
            WriteName(name);
            //			Put(':);
            //			Put(value);
        }

        public void Write<V>(string name, Dictionary<string, V> dict)
        {
            WriteName(name);
            //			Put(':);
            //			Put(value);
        }

        public void Write(string name, params string[] array)
        {
            WriteName(name);
            //			Put(':);
            //			Put(value);
        }

        public void Write(string name, params ISerial[] array)
        {
            WriteName(name);
            //			Put(':);
            //			Put(value);
        }

        public bool Read<T>(ref T value) where T : ISerial, new()
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref char[] value)
        {
            throw new NotImplementedException();
        }
    }
}