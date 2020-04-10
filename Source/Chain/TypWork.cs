using System.Threading.Tasks;
using SkyCloud.Web;

namespace SkyCloud.Chain
{
    [Ui("Types")]
    public class TypWork : WebWork
    {
        public void @default(WebContext wc)
        {
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using var dc = NewDbContext();
                var arr = dc.Query<Typ>("SELECT * FROM chain.typs");

                h.TABLE(arr, o =>
                {
                    h.TD(o.id);
                    h.TD(o.name);
                    h.TD(o.contentyp);
                    h.TD(Typ.Ops[o.op]);
                    h.TD(o.contract == null ? string.Empty : "Contract");
                    h.TD(Typ.Statuses[o.status]);
                });
            });
        }

        [Ui("➕", "Create New Data Type"), Tool(Modal.ButtonShow, Size.Full)]
        public async Task @new(WebContext wc)
        {
            if (wc.IsGet)
            {
                var o = new Typ { };
                o.Read(wc.Query, 0);
                wc.GivePane(200, h =>
                {
                    const string css = "uk-width-medium";
                    h.FORM_().FIELDSUL_("Attributes");
                    h.LI_().LABEL("ID", css).NUMBER(null, nameof(o.id), o.id, min: 1, required: true)._LI();
                    h.LI_().LABEL("Name", css).TEXT(null, nameof(o.name), o.name, min: 2, max: 20, required: true)._LI();
                    h.LI_().LABEL("Content Type", css).TEXT(null, nameof(o.contentyp), o.contentyp, min: 2, max: 20, required: true)._LI();
                    h.LI_().LABEL("Op", css).SELECT(null, nameof(o.op), o.op, Typ.Ops)._LI();
                    h.LI_().LABEL("Contract", css).TEXTAREA(null, nameof(o.contract), o.contract, min: 20, max: 400, required: true)._LI();
                    h.LI_().LABEL("Status", css).SELECT(null, nameof(o.status), o.status, Typ.Statuses, required: true)._LI();
                    h._FIELDSUL()._FORM();
                });
            }
            else // POST
            {
                var o = await wc.ReadObjectAsync<Typ>();
                using var dc = NewDbContext();
                dc.Sql("INSERT INTO chain.datyps")._(o)._VALUES_(o);
                dc.Execute(p => o.Write(p));
                wc.GivePane(200); // created
            }
        }
    }
}