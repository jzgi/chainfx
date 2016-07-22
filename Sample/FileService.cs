using Greatbone.Core;

namespace Greatbone.Sample
{
    public class FileService : WebService
    {
        public FileService(WebCreationContext wcc) : base(wcc)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}