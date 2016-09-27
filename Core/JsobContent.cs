using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// JjavaScript Object Binary
    /// </summary>
    public class JsobContent : DynamicContent, IOut
    {
        // stack of json knots in processing
        readonly int[] starts;

        // current level in stack
        int level;

        // current position
        int pos;


        public JsobContent(int capacity) : base(capacity)
        {
        }


        public override string Type => "application/jsob";



        public IOut Put(string name, bool value)
        {
            throw new NotImplementedException();
        }

        public IOut Put(string name, short value)
        {
            throw new NotImplementedException();
        }

        public IOut Put(string name, int value)
        {
            throw new NotImplementedException();
        }

        public IOut Put(string name, long value)
        {
            throw new NotImplementedException();
        }

        public IOut Put(string name, decimal value)
        {
            throw new NotImplementedException();
        }

        public IOut Put(string name, DateTime value)
        {
            throw new NotImplementedException();
        }

        public IOut Put(string name, char[] value)
        {
            throw new NotImplementedException();
        }

        public IOut Put(string name, string value)
        {
            throw new NotImplementedException();
        }

        public IOut Put(string name, byte[] value)
        {
            throw new NotImplementedException();
        }

        public IOut Put<T>(string name, List<T> value)
        {
            throw new NotImplementedException();
        }

        public IOut Put<T>(string name, Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }

        public IOut Arr(Action a)
        {
            throw new NotImplementedException();
        }

        public IOut Obj(Action a)
        {
            throw new NotImplementedException();
        }

        public IOut Put<T>(string name, T value) where T : IData
        {
            throw new NotImplementedException();
        }
    }
}