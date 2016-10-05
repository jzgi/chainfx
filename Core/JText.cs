using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JText : ISink<JText>
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

        public JText(int capacity)
        {
            buffer = new char[capacity];
        }

        public JText(string json)
        {
            text = json;
        }

        //
        // READ
        //

        public void Arr(Action a)
        {

        }

        public void Obj(Action a)
        {


        }

        

        public JText Put(string name, bool value)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, short value)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, int value)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, long value)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, decimal value)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, DateTime value)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, string value)
        {
            throw new NotImplementedException();
        }

        public JText Put<T>(string name, T value, int x) where T : IPersist
        {
            throw new NotImplementedException();
        }

        public JText Put<T>(string name, List<T> value, int x) where T : IPersist
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, byte[] value)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, JObj value)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, JArr value)
        {
            throw new NotImplementedException();
        }

        public JText PutNull(string name)
        {
            throw new NotImplementedException();
        }


        //
        // WRITE
        //
    }
}