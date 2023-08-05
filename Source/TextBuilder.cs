using System;

namespace ChainFx
{
    /// <summary>
    /// To generate a plain/text string or CSV content. 
    /// </summary>
    public class TextBuilder : ContentBuilder
    {
        public TextBuilder(bool bytely, int capacity) : base(bytely, capacity)
        {
        }

        public override string CType { get; set; } = "text/plain";


        public TextBuilder T(char v)
        {
            Add(v);

            return this;
        }

        public TextBuilder T(bool v)
        {
            Add(v);

            return this;
        }

        public TextBuilder T(short v)
        {
            Add(v);

            return this;
        }

        public TextBuilder T(int v, int digits = 0)
        {
            Add(v, digits);

            return this;
        }

        public TextBuilder T(long v)
        {
            Add(v);

            return this;
        }

        public TextBuilder T(DateTime v, byte date = 3, byte time = 3)
        {
            Add(v, date, time);
            return this;
        }

        public TextBuilder T(decimal v)
        {
            Add(v);

            return this;
        }

        public TextBuilder T(double v)
        {
            Add(v);

            return this;
        }

        public TextBuilder T(string v)
        {
            Add(v);

            return this;
        }

        public TextBuilder T(string v, int offset, int len)
        {
            Add(v, offset, len);

            return this;
        }
    }
}