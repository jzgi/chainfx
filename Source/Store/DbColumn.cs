namespace SkyChain.Store
{
    /// <summary>
    /// The descriptor for a database column. 
    /// </summary>
    public abstract class DbColumn : IData, IKeyable<string>
    {
       
        string column_name;

        string column_default;

        bool is_nullable;

        bool character_maximum_length;

        bool notnull;


        public void Read(ISource s, short proj = 4095)
        {
            s.Get(nameof(column_name), ref column_name);
            s.Get(nameof(is_nullable), ref is_nullable);
            s.Get(nameof(character_maximum_length), ref character_maximum_length);
            s.Get(nameof(notnull), ref notnull);
        }

        public void Write(ISink s, short proj = 4095)
        {
        }

        public string Key => column_name;

        public string Name => column_name;

        public abstract void Convert(ISource src, ISink snk);
    }

    public class SmallintColumn : DbColumn
    {
        public override void Convert(ISource src, ISink snk)
        {
        }
    }

    public class IntColumn : DbColumn
    {
        public override void Convert(ISource src, ISink snk)
        {
        }
    }
}