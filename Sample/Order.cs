using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// An order data object.
    ///
    public class Order : IData
    {
        public const short
            ID = 0x0001,
            CUSTWX = 0x0002,
            LATE = 0x0010,
            DETAIL = 0x0020;

        // state
        public const int
            CREATED = 0,
            RECEIVED = 1,
            SENT = 3,
            DONE = 7,
            ABORTED = 10;

        // status
        static readonly Opt<short> STATUS = new Opt<short>
        {
            [CREATED] = "在购等待收款",
            [RECEIVED] = "到款等待发货",
            [SENT] = "在途",
            [DONE] = "已完成",
            [ABORTED] = "已撤销",
        };


        public static readonly Order Empty = new Order();

        internal long id;
        internal string shopname; // shop name
        internal string shopid;
        internal string custname; // customer name
        internal string custwx; // weixin openid
        internal string custtel; // telephone
        internal string custcity; // city
        internal string custdistr; // disrict
        internal string custaddr; // address
        internal OrderLine[] detail;
        internal decimal total;
        internal string note;
        internal DateTime created; // time created

        internal string prepay_id;
        internal DateTime paid; // time paid

        internal string pack; // packer name
        internal string packtel;
        internal DateTime packed;

        internal string dvrat; // delivered at shopid
        internal string dvr; // deliverer name
        internal string dvrtel;
        internal DateTime dvred;

        internal DateTime closed; // time completed or aborted

        internal short status;

        public void ReadData(IDataInput i, short proj = 0)
        {
            if ((proj & ID) == ID)
            {
                i.Get(nameof(id), ref id);
            }

            i.Get(nameof(shopname), ref shopname);
            i.Get(nameof(shopid), ref shopid);

            i.Get(nameof(custname), ref custname);
            i.Get(nameof(custwx), ref custwx);
            i.Get(nameof(custtel), ref custtel);
            i.Get(nameof(custcity), ref custcity);
            i.Get(nameof(custdistr), ref custdistr);
            i.Get(nameof(custaddr), ref custaddr);
            if ((proj & DETAIL) == DETAIL)
            {
                i.Get(nameof(detail), ref detail);
            }
            i.Get(nameof(total), ref total);
            i.Get(nameof(note), ref note);
            i.Get(nameof(created), ref created);

            if ((proj & LATE) == LATE)
            {
                i.Get(nameof(prepay_id), ref prepay_id);
                i.Get(nameof(paid), ref paid);

                i.Get(nameof(pack), ref pack);
                i.Get(nameof(packtel), ref packtel);
                i.Get(nameof(packed), ref packed);

                i.Get(nameof(dvrat), ref dvrat);
                i.Get(nameof(dvr), ref dvr);
                i.Get(nameof(dvrtel), ref dvrtel);
                i.Get(nameof(dvred), ref dvred);

                i.Get(nameof(closed), ref closed);
            }

            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, short proj = 0) where R : IDataOutput<R>
        {
            if ((proj & ID) == ID)
            {
                o.Put(nameof(id), id, label: "编号");
            }
            o.Group("商家");
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(shopname), shopname);
            o.UnGroup();

            o.Put(nameof(custname), custname, label: "买家");
            if ((proj & CUSTWX) == CUSTWX)
            {
                o.Put(nameof(custwx), custwx);
            }
            o.Put(nameof(custtel), custtel, label: "联系电话");
            o.Group("送货地址");
            o.Put(nameof(custcity), custcity);
            o.Put(nameof(custdistr), custdistr);
            o.Put(nameof(custaddr), custaddr);
            o.UnGroup();

            if ((proj & DETAIL) == DETAIL)
            {
                o.Put(nameof(detail), detail);
            }
            o.Put(nameof(total), total, label: "金额");
            o.Put(nameof(note), note, label: "附加说明");
            o.Put(nameof(created), created, label: "创建时间");

            if ((proj & LATE) == LATE)
            {
                if ((proj & ID) == ID)
                {
                    o.Put(nameof(prepay_id), prepay_id);
                    o.Put(nameof(paid), paid);
                }
                if ((proj & ID) == ID)
                {
                    o.Put(nameof(pack), pack);
                    o.Put(nameof(packtel), packtel);
                    o.Put(nameof(packed), packed);
                }
                if ((proj & ID) == ID)
                {
                    o.Put(nameof(dvrat), dvrat);
                    o.Put(nameof(dvr), dvr);
                    o.Put(nameof(dvrtel), dvrtel);
                    o.Put(nameof(dvred), dvred);
                }
                if ((proj & ID) == ID)
                {
                    o.Put(nameof(closed), closed);
                }
            }
            o.Put(nameof(status), status, label: "状态", opt: STATUS);
        }

        public void AddItem(string item, short qty, string unit, decimal price)
        {
            if (detail == null)
            {
                detail = new[] {new OrderLine() {item = item, qty = qty, unit = unit, price = price}};
            }
            var orderln = detail.Find(o => o.item.Equals(item));
            if (orderln != null)
            {
                orderln.qty += qty;
            }
            else
            {
                detail = detail.Add(new OrderLine() {item = item, qty = qty, unit = unit, price = price});
            }
        }

        public void Sum()
        {
            if (detail != null)
            {
                decimal sum = 0;
                for (int i = 0; i < detail.Length; i++)
                {
                    sum += detail[i].qty * detail[i].price;
                }
                total = sum;
            }
        }
    }

    public class OrderLine : IData
    {
        internal string item;
        internal short qty;
        internal string unit;
        internal decimal price;

        public decimal Subtotal => price * qty;

        public void ReadData(IDataInput i, short proj = 0)
        {
            i.Get(nameof(item), ref item);
            i.Get(nameof(qty), ref qty);
            i.Get(nameof(unit), ref unit);
            i.Get(nameof(price), ref price);
        }

        public void WriteData<R>(IDataOutput<R> o, short proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(item), item, label: "品名");
            o.Group("数量");
            o.Put(nameof(qty), qty);
            o.Put(nameof(unit), unit);
            o.UnGroup();
            o.Put(nameof(price), price, label: "单价");
        }

        public void AddQty(short qty)
        {
            this.qty += qty;
        }
    }
}