using System.IO;

namespace Greatbone.Core
{
    public class EventU
    {
        FileStream stream;

        EventU(
            Service service
        )
        {
            stream = new FileStream(service.Context.GetFilePath(""), FileMode.OpenOrCreate, FileAccess.ReadWrite);

        }

        public int GetPointer(string moniker)
        {
            byte[] buf = new byte[21];
            stream.Read(buf, 0, buf.Length);
            return 0;
        }

        public void Write(int pointer, long id)
        {

        }
    }
}
