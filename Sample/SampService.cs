using Greatbone;

namespace Samp
{
    /// <summary>
    /// The sample service that hosts all official accounts.
    /// </summary>
    public class SampService : Service
    {
        public SampService(ServiceConfig cfg) : base(cfg)
        {
            CreateVar<SampVarWork, string>(obj => ((Item)obj).name);

            // register cached active hubs
            Register(delegate
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT ").collst(Hub.Empty).T(" FROM hubs WHERE status > 0 ORDER BY name");
                        return dc.Query<string, Hub>(proj: 0xff);
                    }
                }, 3600
            );

            // register cached active orgs
            Register(delegate
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT ").collst(Org.Empty).T(" FROM orgs WHERE status > 0 ORDER BY hubid, name");
                        return dc.Query<(string, short), Org>(proj: 0xff);
                    }
                }, 300
            );
        }
    }
}