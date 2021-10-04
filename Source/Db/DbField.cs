namespace SkyChain.Db
{
    /// <summary>
    /// A database column or argument field. 
    /// </summary>
    public class DbField : IKeyable<string>
    {
        readonly DbTyp type;

        readonly string name;

        readonly char mode;

        readonly uint typoid;

        readonly bool def;

        readonly bool notnull;


        internal DbField(DbContext s)
        {
            s.Get(nameof(name), ref name);

            s.Get(nameof(typoid), ref typoid);

            s.Get(nameof(def), ref def);

            s.Get(nameof(notnull), ref notnull);

            type = DbTyp.GetBaseType(typoid);
        }

        public DbField(char mode, string name, uint typoid, bool def)
        {
            this.mode = mode;
            this.name = name;
            this.typoid = typoid;
            this.def = def;

            type = DbTyp.GetBaseType(typoid);
        }

        public string Key => name;

        public string Name => name;

        public DbTyp Type => type;

        public void Convert(ISource src, ISink snk)
        {
            type?.Converter(name, src, snk);
        }
    }
}