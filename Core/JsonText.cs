using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JsonText : ISerialReader, ISerialWriter
    {
        // for parsing json text
        string text;

        // for building json text
        char[] buffer;

        int count;

        int pos;

        // parsing context for levels

        int[] starts = new int[8];

        int level;

        public JsonText(int capacity)
        {
            buffer = new char[capacity];
        }

        public JsonText(string json)
        {
            text = json;
        }

        //
        // READ
        //

        public bool Array(Action a)
        {
            // enter the openning bracket
            int p = pos;
            for (;;)
            {
                p++;
                if (p >= count) return false;
                char c = buffer[p];
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                {
                    continue;
                }
                if (c == '[')
                {
                    level++;
                    starts[level] = p;
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
                char c = buffer[p++];
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

        public bool Object(Action a)
        {
            // enter the openning bracket
            int p = pos;
            for (;;)
            {
                p++;
                if (p >= count) return false;
                char c = buffer[p];
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                {
                    continue;
                }
                if (c == '{')
                {
                    level++;
                    starts[level] = p;
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
                char c = buffer[p++];
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

        public bool Read(ref bool value)
        {
            int p = pos;
            char c;
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
            throw new NotImplementedException();
        }

        public bool Read(ref int value)
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

        public bool Read(ref byte[] value)
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

        public bool Read(string name, ref byte[] value)
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

        public void Write(byte[] value)
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

        public void Write(string name, bool value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, short value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, int value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, long value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, decimal value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, DateTime value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, char[] value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, string value)
        {
            throw new NotImplementedException();
        }

        public void Write<T>(string name, T value) where T : ISerial
        {
            throw new NotImplementedException();
        }

        public void Write(string name, byte[] value)
        {
            throw new NotImplementedException();
        }

        public void Write<T>(string name, List<T> value)
        {
            throw new NotImplementedException();
        }

        public void Write<T>(string name, Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }

        //
        // WRITE
        //
    }
}