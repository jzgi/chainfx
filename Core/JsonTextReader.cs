using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JsonTextReader : ISerialReader
    {
        // for input
        private string text;

        private int pos;

        // parsing context for levels

        JsonKnot[] levels = new JsonKnot[8];

        int level;

        public JsonTextReader(string str)
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

        public bool Read(ref short value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref long value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref decimal value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref DateTime value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref char[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref string value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(ref T value) where T : ISerial, new()
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(ref T[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(ref List<T> value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(ref Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref bool value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref short value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref int value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref long value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref decimal value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref DateTime value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref char[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref string value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(string name, ref T value) where T : ISerial, new()
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