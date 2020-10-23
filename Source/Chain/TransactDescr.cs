namespace SkyChain.Chain
{
    public class TransactDescr : IKeyable<short>
    {
        short typ;

        string name;

        Transit transit;

        Consent consent;

        public TransactDescr(short typ, string name, Transit transit, Consent consent)
        {
            this.typ = typ;
            this.name = name;
            this.transit = transit;
            this.consent = consent;
        }

        public short Key => typ;
    }
}