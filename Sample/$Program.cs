using Greatbone.Core;

namespace Greatbone.Sample
{
    //
    // hide sensitive information from the public
    //

    public partial class Program
    {
        static DbConfig pg = new DbConfig
        {
            host = "106.14.45.109",
            port = 5432,
            username = "postgres",
            password = "GangShang721004",
            queue = false
        };
    }
}