namespace Greatbone.Core
{
    public class WebCreationContext
    {
        public string Key { get; set; }

        public string StaticPath { get; set; }

        public IParent Parent { get; set; }

        public WebService Service { get; }
    }
}