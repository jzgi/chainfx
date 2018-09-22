using Greatbone;

namespace Samp
{
    /// <summary>
    /// The sample service that hosts all regional operations.
    /// </summary>
    [Ui("全粮派")]
    public class SampService : Service
    {
        public SampService(ServiceConfig cfg) : base(cfg)
        {
            CreateVar<SampVarWork, string>(obj => ((Item) obj).name);

            // register a cachable map of active regions
            Register(delegate
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT ").collst(Reg.Empty).T(" FROM regs WHERE status > 0 ORDER BY name");
                        return dc.Query<string, Reg>(proj: 0xff);
                    }
                }, 1800
            );

            // register a cachable map of active orgs
            Register(delegate
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT ").collst(Org.Empty).T(" FROM orgs WHERE status > 0 ORDER BY regid, name");
                        return dc.Query<string, Org>(proj: 0xff);
                    }
                }, 1800
            );
        }
    }
}