using System.Threading.Tasks;
using CloudUn.Web;

namespace CloudUn.Net
{
    [Ui("Data Types")]
    public class DataTypWork : WebWork
    {
        public void @default(WebContext wc)
        {
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using var dc = NewDbContext();
                var arr = dc.Query<DataTyp>("SELECT * FROM chain.datyps");

                h.TABLE(arr, o =>
                {
                    h.TD(o.id);
                    h.TD(o.name);
                    h.TD(DataTyp.Ops[o.op]);
                    h.TD(DataTyp.Statuses[o.status]);
                });
            });
        }

        [Ui("New", "Create New Data Type"), Tool(Modal.ButtonShow)]
        public async Task @new(WebContext wc)
        {
            if (wc.IsGet)
            {
                var o = new DataTyp { };

                o.Read(wc.Query, 0);
                wc.GivePane(200, h =>
                {
                    const string css = "uk-width-small";
                    h.FORM_().FIELDSUL_("数据类型参数");
                    h.LI_().LABEL("ID", css).NUMBER(null, nameof(o.id), o.id, min: 1, required: true)._LI();
                    h.LI_().LABEL("Name", css).TEXT(null, nameof(o.name), o.name, min: 2, max: 20, required: true)._LI();
                    h.LI_().LABEL("Operation", css).SELECT(null, nameof(o.op), o.op, DataTyp.Ops)._LI();
                    h.LI_().LABEL("Status", css).SELECT(null, nameof(o.status), o.status, DataTyp.Statuses, required: true)._LI();
                    h._FIELDSUL()._FORM();
                });
            }
            else // POST
            {
                var o = await wc.ReadObjectAsync<DataTyp>();
                using var dc = NewDbContext();
                dc.Sql("INSERT INTO chain.datyps")._(o)._VALUES_(o);
                dc.Execute(p => o.Write(p));
                wc.GivePane(200); // created
            }
        }
    }
}