namespace Greatbone.Core
{
    public class WebArg
    {
        internal string key;


        public string Key => key;

        public bool Auth { get; internal set; }

        public bool IsVar { get; internal set; }

        public IParent Parent { get; internal set; }

        public virtual string Folder { get; internal set; }

        public WebService Service { get; internal set; }

    }
}