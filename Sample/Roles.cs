using Greatbone;
using static Samp.User;

namespace Samp
{
    [UserAccess(true)]
    public class MyWork : Work
    {
        public MyWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<MyVarWork, int>((obj) => ((User) obj).id);
        }
    }

    [UserAccess(hub: 1)]
    [Ui("主页")]
    public class HubWork : Work
    {
        public HubWork(WorkConfig cfg) : base(cfg)
        {
            Create<HubOrderWork>("order");

            Create<HubItemWork>("item");

            Create<HubChatWork>("chat");

            Create<HubUserWork>("user");

            Create<HubOrgWork>("org");

            Create<HubRepayWork>("repay");
        }

        public void @default(WebContext wc)
        {
            bool inner = wc.Query[nameof(inner)];
            if (inner)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT * FROM users WHERE ctr > 0 ORDER BY ctr");
                    var arr = dc.Query<User>();
                    wc.GivePage(200, h =>
                    {
                        h.TOOLBAR();
                        h.TABLE(arr, null,
                            o => h.TD(o.name).TD(o.tel).TD(Hubly[o.hub])
                        );
                    });
                }
            }
            else
            {
                wc.GiveFrame(200, false, 60 * 15, "调度作业");
            }
        }
    }

    public class ShopWork : Work
    {
        public ShopWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<ShopVarWork, string>(prin => ((User) prin).shopat);
        }
    }


    public class TeamWork : Work
    {
        public TeamWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<TeamVarWork, string>(prin => ((User) prin).teamat);
        }
    }
}