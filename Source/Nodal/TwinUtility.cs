namespace ChainFx.Nodal
{
    public static class NodalUtility
    {
        public const string X_FROM = "X-From";

        public const string X_CRYPTO = "X-Peer-ID";

        public const string X_BLOCK_ID = "X-Block-ID";

        public const string X_ACCOUNT = "X-Account";

        public const string X_NAME = "X-Name";

        public const string X_DUTY = "X-Duty";

        public const string X_OP = "X-Op";


        public const int BLOCK_CAPACITY = 1000;

        internal static (int blockid, short idx) ResolveSeq(long seq)
        {
            var blockid = (int)(seq / BLOCK_CAPACITY);
            var idx = (short)(seq % BLOCK_CAPACITY);
            return (blockid, idx);
        }

        internal static long WeaveSeq(int blockid, short idx)
        {
            return (long)blockid * BLOCK_CAPACITY + idx;
        }
    }
}