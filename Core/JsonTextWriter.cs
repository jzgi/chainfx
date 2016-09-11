using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JsonTextWriter : ISerialWriter
    {
        // for output
        private char[] buffer;

        private int pos;

        // parsing context for levels

        JsonKnot[] levels = new JsonKnot[8];

        int level;

        public JsonTextWriter(int capacity)
        {
        }

        public bool WriteArray(Action a)
        {
            throw new NotImplementedException();
        }

        public bool WriteObject(Action a)
        {
            throw new NotImplementedException();
        }

        public bool Write(bool value)
        {
            throw new NotImplementedException();
        }

        public bool Write(short value)
        {
            throw new NotImplementedException();
        }

        public bool Write(int value)
        {
            throw new NotImplementedException();
        }

        public bool Write(long value)
        {
            throw new NotImplementedException();
        }

        public bool Write(decimal value)
        {
            throw new NotImplementedException();
        }

        public bool Write(DateTime value)
        {
            throw new NotImplementedException();
        }

        public bool Write(char[] value)
        {
            throw new NotImplementedException();
        }

        public bool Write(string value)
        {
            throw new NotImplementedException();
        }

        public bool Write<T>(T value) where T : ISerial
        {
            throw new NotImplementedException();
        }

        public bool Write<T>(T[] value)
        {
            throw new NotImplementedException();
        }

        public bool Write<T>(List<T> value)
        {
            throw new NotImplementedException();
        }

        public bool Write<T>(Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }

        public bool Write(string name, bool value)
        {
            throw new NotImplementedException();
        }

        public bool Write(string name, short value)
        {
            throw new NotImplementedException();
        }

        public bool Write(string name, int value)
        {
            throw new NotImplementedException();
        }

        public bool Write(string name, long value)
        {
            throw new NotImplementedException();
        }

        public bool Write(string name, decimal value)
        {
            throw new NotImplementedException();
        }

        public bool Write(string name, DateTime value)
        {
            throw new NotImplementedException();
        }

        public bool Write(string name, char[] value)
        {
            throw new NotImplementedException();
        }

        public bool Write(string name, string value)
        {
            throw new NotImplementedException();
        }

        public bool Write<T>(string name, T value) where T : ISerial
        {
            throw new NotImplementedException();
        }

        public bool Write<T>(string name, T[] value)
        {
            throw new NotImplementedException();
        }

        public bool Write<T>(string name, List<T> value)
        {
            throw new NotImplementedException();
        }

        public bool Write<T>(string name, Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }
    }
}