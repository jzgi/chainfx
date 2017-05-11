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

        [Ui("修改", Mode = UiMode.ButtonDialog)]
        public async Task edit(ActionContext ac)
        {
            string name = ac[this];
            long id = ac[-1];

            bool remove = false;

            if (ac.GET)
            {
                const int proj = -1 ^ Order.ID ^ Order.LATE;
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT shopid, detail, total FROM orders WHERE id = @1", p => p.Set(id)))
                    {
                        var shopid = dc.GetString();
                        var detail = dc.GetDatas<OrderLine>();
                        var ln = detail.Find(x => x.name == name);

                        string unit = null;
                        decimal price = 0;
                        short min = 0;
                        short step = 0;
                        if (dc.Query1("SELECT unit, price, min, step FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(ln.name)))
                        {
                            unit = dc.GetString();
                            price = dc.GetDecimal();
                            min = dc.GetShort();
                            step = dc.GetShort();
                        }


                        ac.GivePane(200, m =>
                        {
                            m.FORM_();

                            m.FIELDSET_("修改数量");

                            m.Add("<div class=\"row card align-middle\">");

                            m.Add("<div class=\"small-6 column\">");
                            m.Add("<img src=\"/pub/");
                            m.Add(shopid);
                            m.Add("/");
                            m.Add(name);
                            m.Add("/icon\" class=\"thumbnail\">");
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
                        var o = new Order()
                        {
                            shopid = dc.GetString(),
                            detail = dc.GetDatas<OrderLine>(),
                            total = dc.GetDecimal()
                        };
                        if (remove)
                        {
                            o.RemoveLine(name);
                            if (o.detail == null) // delete the whole record if cart is empty
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
                        dc.Execute("UPDATE orders SET detail = @1, total = @2 WHERE id = @3", p => p.Set(o.detail).Set(o.total).Set(id));
                    }
                }
                Redirect:
                ac.GiveRedirect("../../");
            }
        }
    }
}