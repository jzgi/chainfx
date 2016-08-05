
namespace Greatbone.Core
{
    public static class WebResponseExtensions
    {
        public static void Output(this WebResponse o, IData data, int flags)
        {
            o.PutStart();

            data.To(o, flags);

            o.PutEnd();
        }
    }
}