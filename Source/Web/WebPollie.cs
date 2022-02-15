namespace SkyChain.Web
{
    public class WebPollie
    {
        public const int MAX = 32;

        readonly string[] array = new string[MAX];

        int head;

        int tail;

        int count;

        internal int Tick { get; set; }
    }
}