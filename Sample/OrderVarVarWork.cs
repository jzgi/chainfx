using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
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

        [Ui("修改")]
        public void edit(ActionContext ac)
        {
        }

        [Ui("删除")]
        public void rm(ActionContext ac)
        {
        }
    }
}