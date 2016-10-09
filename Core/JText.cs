using System;

namespace Greatbone.Core
{
    public class JText : ISink<JText>
    {
        const int InitialCapacity = 1024;

        char[] buffer;

        int count;

        int pos;

        // parsing context for levels

        int[] starts = new int[8];

        int level;

        public JText(int capacity = InitialCapacity)
        {
            buffer = new char[capacity];
        }

        public void Arr(Action a)
        {

        }

        public void Obj(Action a)
        {


        }

        public JText Put(JArr v)
        {
            throw new NotImplementedException();
        }

        public JText PutNull(string name)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, bool v)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, short v)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, int v)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, long v)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, decimal v)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, DateTime v)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, char[] v)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, string v)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, byte[] v)
        {
            throw new NotImplementedException();
        }

        public JText Put<T>(string name, T v, int x = -1) where T : IPersist
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, JObj v)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, JArr v)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, short[] v)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, int[] v)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, long[] v)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, string[] v)
        {
            throw new NotImplementedException();
        }

        public JText Put<T>(string name, T[] v, int x = -1) where T : IPersist
        {
            throw new NotImplementedException();
        }
    }
}