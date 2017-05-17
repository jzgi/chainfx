using Greatbone.Core;

namespace Greatbone.Sample
{
    [User]
    public abstract class CityVarWork : Work
    {
        protected CityVarWork(WorkContext wc) : base(wc)
        {
        }

        public void _icon_(ActionContext ac)
        {
            string shopid = ac[this];

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM shops WHERE id = @1", p => p.Set(shopid)))
                {
                    var byteas = dc.GetByteAS();
                    if (byteas.Count == 0) ac.Give(204); // no content 
                    else
                    {
                        StaticContent cont = new StaticContent(byteas);
                        ac.Give(200, cont);
                    }
                }
                else ac.Give(404); // not found           
            }
        }
    }


    [Ui("设置")]
    public class SprCityVarWork : CityVarWork
    {
        public SprCityVarWork(WorkContext wc) : base(wc)
        {
            Create<SprUserWork>("user");

            Create<SprShopWork>("shop");

            Create<SprRepayWork>("present");

            Create<SprPastRepayWork>("past");
        }

        public void @default(ActionContext ac)
        {
            ac.GiveFrame(200);
        }
    }
}