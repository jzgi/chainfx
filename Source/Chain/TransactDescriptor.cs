namespace Skyiah.Chain
{
    public class TransactDescriptor : IKeyable<short>
    {
        short typ;

        string name;

        Transit transit;

        Consent consent;

        public TransactDescriptor(short typ, string name, Transit transit, Consent consent)
        {
            this.typ = typ;
            this.name = name;
            this.transit = transit;
            this.consent = consent;
        }

        public short Key => typ;
    }
}