namespace SkyCloud.Chain
{
    public class TransactDescriptor : IKeyable<short>
    {
        short typ;

        string name;

        Transiter transiter;

        Consenter consenter;

        public short Key => typ;
    }
}