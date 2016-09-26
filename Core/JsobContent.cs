using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// JjavaScript Object Binary
    /// </summary>
    public class JsobContent : DynamicContent, ISerialReader, ISerialWriter
    {
        // stack of json knots in processing
        readonly int[] starts;

        // current level in stack
        int level;

        // current position
        int pos;


        public JsobContent(byte[] buf, int count) : base(buf, count)
        {
            starts = new int[8];
            level = -1;
            pos = -1;
        }


        public JsobContent(int capacity) : base(capacity)
        {
        }


        public override string Type => "application/jsob";


        public bool Array(Action a)
        {
            int p = pos;
            p++;
            if (buffer[p] != '[')
            {
                return false;
            }

            return false;
        }

        public bool Object(Action a)
        {
            int p = pos;
            p++;
            if (buffer[p] != '[')
            {
                return false;
            }

            return false;
        }

        public bool Read(ref bool value)
        {
            throw new NotImplementedException();
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

        public void Write(byte[] value)
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
    }
}