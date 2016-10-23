using System;

namespace Greatbone.Core
{
    public abstract class MaterialHtContent : HtContent<MaterialHtContent>, ISink<MaterialHtContent>
    {
        public MaterialHtContent(int capacity) : base(capacity)
        {
        }

        public void Page()
        {
            T("<!doctype html>");
            T("<html>");

            T("<body>");

            T("</body>");
            T("</html>");
        }

        public void dialog(string h, Action content)
        {
            T("<dialog class=\"mdl-dialog\">");
            T("<h4 class=\"mdl-dialog__title\">").T(h).T("</h4>");

            T("<div class=\"mdl-dialog__content\">");

            T("<div class=\"mdl-dialog__actions\">");
            T("<button type=\"button\" class=\"mdl-button\">Agree</button>");
            T("<button type=\"button\" class=\"mdl-button close\">Disagree</button>");
            T("</div>");

            T("</div>");
            T("</dialog>");

            T("<script>");


            T("</script>");
        }


        public void table<M>(M[] arr, Action content) where M : IPersist
        {
            T("<table class=\"mdl-data-table mdl-js-data-table mdl-shadow--2dp\">");
            T("<thead>");

            M obj = arr[0];


            T("<th class=\"mdl-data-table__cell--non-numeric\">Material</th>");
            T("<th>Quantity</th>>");

            T("</thead>");
            T("<tbody>");

            for (int i = 0; i < arr.Length; i++)
            {
                T("<tr>");
                T("<td class=\"mdl-data-table__cell--non-numeric\">Acrylic (Transparent)</td>");
                T("<td>25</td>");

                T("</tr>");
            }

            T("</tbody>");

            T("</table>");
        }


        public MaterialHtContent PutNull(string name)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put(string name, bool v)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put(string name, short v)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put(string name, int v)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put(string name, long v)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put(string name, decimal v)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put(string name, Number v)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put(string name, DateTime v)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put(string name, char[] v)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put(string name, string v)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put(string name, byte[] v)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put(string name, ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put<V>(string name, V v, uint x = 0) where V : IPersist
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put(string name, JObj v)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put(string name, JArr v)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put(string name, short[] v)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put(string name, int[] v)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put(string name, long[] v)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put(string name, string[] v)
        {
            throw new NotImplementedException();
        }

        public MaterialHtContent Put<V>(string name, V[] v, uint x = 0) where V : IPersist
        {
            throw new NotImplementedException();
        }
    }
}