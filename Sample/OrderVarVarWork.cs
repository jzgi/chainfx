using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    public abstract class OrderVarVarWork : Work
    {
        protected OrderVarVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class MyCartOrderVarVarWork : OrderVarVarWork
    {
        public MyCartOrderVarVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("修改", Mode = UiMode.ButtonPrompt)]
        public async Task edit(ActionContext ac)
        {
            string name = ac[this];
            long id = ac[-1];

            bool remove = false;
            string shopid;
            Detail[] detail;

            if (ac.GET)
            {
                const int proj = 0x00ff ^ Order.ID;
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT shopid, detail, total FROM orders WHERE id = @1", p => p.Set(id)))
                    {
                        dc.Let(out shopid).Let(out detail);

                        var ln = detail.Find(x => x.name == name);

                        string unit = null;
                        decimal price = 0;
                        short min = 0;
                        short step = 0;
                        if (dc.Query1("SELECT unit, price, min, step FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(ln.name)))
                        {
                            dc.Let(out unit).Let(out price).Let(out min).Let(out step);
                        }


                        ac.GivePane(200, m =>
                        {
                            m.FORM_();

                            m.FIELDSET_("修改数量");

                            m.Add("<div class=\"row align-middle\">");

                            m.Add("<div class=\"small-6 column\">");
                            m.Add("<img src=\"/pub/");
                            m.Add(shopid);
                            m.Add("/");
                            m.Add(name);
                            m.Add("/icon\" class=\"thumbnail circle\">");
                            m.Add("</div>"); // column

                            m.Add("<div class=\"small-6 column\">");
                            m.Add("<h3>");
                            m.Add(name);
                            m.Add("</h3>");
                            m.Add("<p>");
                            m.Add("<strong class=\"money\">&yen;");
                            m.Add(price);
                            m.Add("</strong> 每");
                            m.Add(unit);
                            m.Add("</p>");
                            m.NUMBER(nameof(ln.qty), ln.qty, min: min, step: step);
                            m.Add("</div>");

                            m.Add("</div>");

                            m._FIELDSET();

                            m.FIELDSET_("删除");
                            m.CHECKBOX(nameof(remove), remove, label: "从购物车删除");
                            m._FIELDSET();

                            m._FORM();
                        });
                    }
                }
            }
            else // post
            {
                var f = await ac.ReadAsync<Form>();
                short qty = f[nameof(qty)];
                remove = f[nameof(remove)];

                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT shopid, detail, total FROM orders WHERE id = @1", p => p.Set(id)))
                    {
                        var o = new Order();
                        dc.Let(out o.shopid).Let<Detail>(out o.details).Let(out o.total);

                        if (remove)
                        {
                            o.RemoveLine(name);
                            if (o.details == null) // delete the whole record if cart is empty
                            {
                                dc.Execute("DELETE FROM orders WHERE id = @1", p => p.Set(id));
                                goto Redirect;
                            }
                        }
                        else
                        {
                            o.SetLineQty(name, qty);
                        }
                        o.Sum();
                        dc.Execute("UPDATE orders SET detail = @1, total = @2 WHERE id = @3", p => p.Set(o.details).Set(o.total).Set(id));
                    }
                }
                Redirect:
                ac.GiveRedirect("../../");
            }
        }
    }
}