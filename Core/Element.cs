using System;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// A form object model parsed from x-www-form-urlencoded.
    /// </summary>
    ///
    public class Element : ISource
    {
        const int InitialCapacity = 16;

        Roll<Attr> attrs;

        string text;

        Element[] children;

        int count;

        public Element(int capacity = InitialCapacity)
        {
        }

        internal void Add(string name, string v)
        {
            Attr pair = new Attr(name, v);
            attrs.Add(pair);
        }

        public bool Get(string name, ref bool v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref int v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref long v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref decimal v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref Number v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref DateTime v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref char[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref string v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref byte[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref ArraySegment<byte>? v)
        {
            throw new NotImplementedException();
        }

        public bool Get<V>(string name, ref V v, byte z = 0) where V : IPersist, new()
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref JObj v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref JArr v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref int[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref long[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref string[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get<V>(string name, ref V[] v, byte z = 0) where V : IPersist, new()
        {
            throw new NotImplementedException();
        }

        public int Count => attrs.Count;

        public Attr this[int index] => attrs[index];

        public Attr this[string name] => attrs[name];


        //
        // SOURCE
        //

    }

}