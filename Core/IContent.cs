
namespace Greatbone.Core
{
    public interface IContent
    {
        /// <summary>Content-Type</summary>
        ///
        string Type { get; }

        /// <summary>The byte buffer that contains the content.</summary>
        ///
        byte[] ByteBuffer { get; }

        char[] CharBuffer { get; }

        bool IsBinary { get; }

        /// <summary>The number of bytes.</summary>
        ///
        int Length { get; }

    }

}