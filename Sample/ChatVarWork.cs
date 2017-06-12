using System;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    [User]
    public abstract class ChatVarWork : Work
    {
        protected ChatVarWork(WorkContext wc) : base(wc)
        {
        }

        public void _icon_(ActionContext ac)
        {
            string shopid = ac[this];

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM shops WHERE id = @1", p => p.Set(shopid)))
                {
                    ArraySegment<byte> byteas;
                    dc.Let(out byteas);
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

    public class OprChatVarWork : ChatVarWork
    {
        public OprChatVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("修改", Mode = UiMode.ButtonShow)]
        public async Task edit(ActionContext ac)
        {
        }
    }
}