using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JsonText : ISerialReader, ISerialWriter
    {

       private string text;

        // for output
        private char[] buffer;

        private int pos;

        // parsing context for levels

        JsonLR[] levels = new JsonLR[8];

        int level;

        public JsonText(int capacity)
        {
        }

        public JsonText(string str)
        {
            this.text = str;
        }

        //
        // READ
        //

        public bool ReadArray(Action a)
        {
            throw new NotImplementedException();
        }

        public bool ReadObject(Action a)
        {
            throw new NotImplementedException();
        }

        public bool Read(out bool value)
        {
            throw new NotImplementedException();
        }

        public bool Read(out short value)
        {
            throw new NotImplementedException();
        }

        public bool Read(out int value)
        {
            throw new NotImplementedException();
        }

        public bool Read(out long value)
        {
            throw new NotImplementedException();
        }

        public bool Read(out decimal value)
        {
            throw new NotImplementedException();
        }

        public bool Read(out DateTime value)
        {
            throw new NotImplementedException();
        }

        public bool Read(out char[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read(out string value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(out T value) where T : ISerial, new()
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(out T[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(out List<T> value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(out Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, out bool value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, out short value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, out int value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, out long value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, out decimal value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, out DateTime value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, out char[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, out string value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(string name, out T value) where T : ISerial, new()
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(string name, out T[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(string name, out List<T> value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(string name, out Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }

        public void WriteArray(Action a)
        {
            throw new NotImplementedException();
        }

        public void WriteObject(Action a)
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

        public void Write<T>(string name, T[] value)
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