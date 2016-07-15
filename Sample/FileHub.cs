using Greatbone.Core;

namespace Greatbone.Sample
{
    public class FileHub : WebHub
    {
        public FileHub(WebHub hub) : base(hub)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}