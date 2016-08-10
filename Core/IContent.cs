namespace Greatbone.Core
{
    public interface IContent 
    {

        string MimeType();

        byte[] Buffer();

        int Offset();

        int Count();

    }
}