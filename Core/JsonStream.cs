using System.Collections.Generic;
using System.IO;

namespace Greatbone.Core
{
    public class JsonStream : IInput
    {
        private Stream stream;

        public bool Got(string name, out int value)
        {
            throw new System.NotImplementedException();
        }

        public bool Got(string name, out decimal value)
        {
            throw new System.NotImplementedException();
        }

        public bool Got(string name, out string value)
        {
            throw new System.NotImplementedException();
        }

        public bool Got<T>(string name, out List<T> value) where T : IDump
        {
            throw new System.NotImplementedException();
        }

        public bool GotStart()
        {
            throw new System.NotImplementedException();
        }

        public bool GotEnd()
        {
            throw new System.NotImplementedException();
        }
    }
}