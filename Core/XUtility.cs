
namespace Greatbone.Core
{
    public static class XUtility
    {

        public const uint NoDefault = 2, NoBinary = 4, NoExtra = 8;
        

        public static bool Default(this uint x)
        {
            return (x & NoDefault) != NoDefault;
        }

        public static bool Binary(this uint x)
        {
            return (x & NoBinary) != NoBinary;
        }

        public static bool Extra(this uint x)
        {
            return (x & NoExtra) != NoExtra;
        }

    }
}