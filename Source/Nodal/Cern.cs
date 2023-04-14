namespace ChainFx.Nodal
{
    public class Cern : IKeyable<string>
    {
        private string id;


        public static readonly Map<string, Cern> Typs = new Map<string, Cern>
        {
        };


        public string Key => id;
    }
}