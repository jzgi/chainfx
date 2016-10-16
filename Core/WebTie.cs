namespace Greatbone.Core
{
    public class WebTie : ISetting
    {
        internal string key;


        public string Key => key;

        public bool AuthRequired { get; internal set; }

        public bool IsVar { get; internal set; }

        public IParent Parent { get; internal set; }

        public string Folder { get; internal set; }

        public WebService Service { get; internal set; }
    }
}