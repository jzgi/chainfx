using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Pay : IData
    {
        internal string mch_id;
        internal string openid;
        internal string bank_type;
        internal string total_fee;
        internal string transaction_id; // 微信支付订单号
        internal string out_trade_no; // 商户订单号

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(mch_id), ref mch_id);
            i.Get(nameof(openid), ref openid);
            i.Get(nameof(bank_type), ref bank_type);
            i.Get(nameof(total_fee), ref total_fee);
            i.Get(nameof(transaction_id), ref transaction_id);
            i.Get(nameof(out_trade_no), ref out_trade_no);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(mch_id), mch_id);
            o.Put(nameof(openid), openid);
            o.Put(nameof(bank_type), bank_type);
            o.Put(nameof(total_fee), total_fee);
            o.Put(nameof(transaction_id), transaction_id);
            o.Put(nameof(out_trade_no), out_trade_no);
        }

    }

}