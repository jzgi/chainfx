using Greatbone.Core;

namespace Greatbone.Sample
{
    public class FileService : WebService
    {
        public FileService(WebService service) : base(service)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}