using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Brand : IData
    {
        public string Id;

        public string Name;

        public char[] Credential { get; set; }

        public long ModifiedOn { get; set; }

        public string Key => Id;

        public void In(IDataIn i)
        {
        }

        public void Out<R>(IDataOut<R> o) where R : IDataOut<R>
        {
        }
    }
}