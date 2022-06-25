using System;

namespace CoChain.Web
{
    public class SvgChart<S> : IResource where S : struct
    {
        const int INITIAL_CAPACITY = 16, MAX_CAPACITY = 128;

        static readonly string[] COLORS = {"olive", "chocolate", "teal", "navy", "purple"};

        // names of datasets
        readonly string[] sets;

        // values can be uneven
        readonly decimal[] marks;

        // to determine which column a period shoud sits
        readonly Func<S, int> columnof;

        decimal[,] data;

        S[] segs;

        // actual columns of data
        int count;

        public SvgChart(string[] sets, decimal[] marks, Func<S, int> columnof)
        {
            if (sets.Length > COLORS.Length)
            {
                throw new ArgumentException("too many datasets");
            }
            this.sets = sets;
            segs = new S[INITIAL_CAPACITY];
            data = new decimal[INITIAL_CAPACITY, sets.Length];

            this.marks = marks;
            this.columnof = columnof;
        }

        public string[] Sets => sets;

        private int Count => count;

        public void Add(int set, S seg, decimal v)
        {
            // get index of segment
            var idx = columnof(seg);
            if (idx >= MAX_CAPACITY)
            {
                throw new ArgumentException("segment too large");
            }
            // ensure capacity
            var ocap = data.GetLength(0); // old cap
            if (idx >= ocap)
            {
                var ncap = ocap;
                while (ncap <= idx)
                {
                    ncap *= 2;
                }
                // enlarge data
                var ndata = new decimal[ncap, sets.Length];
                Array.Copy(data, 0, ndata, 0, ocap);
                data = ndata;
                // enlarge segs
                var nsegs = new S[ncap];
                Array.Copy(segs, 0, nsegs, 0, ocap);
                segs = nsegs;
            }

            // put value
            data[idx, set] = v;
            segs[idx] = seg;
            // adjust actual columns
            if (idx > count)
            {
                count = idx;
            }
        }

        public Action<S, int> PutLabel { get; set; }

        const int
            margin = 8, maxx = 400, maxy = 200;

        const int
            markwid = 28, legendhei = 20, labelhei = 30;

        const int left = margin + markwid;
        const int right = maxx - margin;
        const int bottom = maxy - margin - legendhei - labelhei;
        const int top = margin;

        public void Write<C>(C cnt) where C : DynamicContent, ISink
        {
            cnt.Add("<svg height=\"100%\" width=\"100%\" viewBox=\"0 0 ");
            cnt.Add(maxx);
            cnt.Add(' ');
            cnt.Add(maxy);
            cnt.Add("\" preserveAspectRatio=\"none\">");

            WriteGrid(cnt);
            WriteLegend(cnt);

            var vspan = marks[^1] - marks[0];

            // draw axies
            var vper = (bottom - top) / vspan;


            if (count > 0)
            {
                var hper = (right - left) / count;

                // print points
                for (int i = 0; i < sets.Length; i++)
                {
                    // path
                    cnt.Add("<polyline stroke=\"");
                    cnt.Add(COLORS[i]);
                    cnt.Add("\" stroke-width=\"2\" fill=\"none\" points=\"");
                    for (int k = 0; k < count; k++)
                    {
                        var v = data[k, i];
                        if (v == 0)
                        {
                            continue;
                        }
                        if (k > 0)
                        {
                            cnt.Add(' ');
                        }
                        int x = left + k * hper + hper / 2;
                        cnt.Add(x);
                        cnt.Add(',');
                        int y = (int) (top + (marks[^1] - v) * vper);
                        cnt.Add(y);
                    }
                    cnt.Add("\"/>");
                    // little circles
                    cnt.Add("<g stroke=\"white\" stroke-width=\"2\" fill=\"");
                    cnt.Add(COLORS[i]);
                    cnt.Add("\">");
                    for (int k = 0; k < count; k++)
                    {
                        var v = data[k, i];
                        if (v == 0)
                        {
                            continue;
                        }
                        cnt.Add("<circle cx=\"");
                        int cx = left + k * hper + hper / 2;
                        cnt.Add(cx);
                        cnt.Add("\" cy=\"");
                        int cy = (int) (top + (marks[^1] - v) * vper);
                        cnt.Add(cy);
                        cnt.Add("\" r=\"4\"/>");
                    }
                    cnt.Add("</g>");
                }
            }
            cnt.Add("</svg>");
        }

        public void WriteGrid<C>(C cnt) where C : DynamicContent, ISink
        {
            var vspan = marks[^1] - marks[0];

            // draw axies
            var vper = (bottom - top) / vspan;

            // marks
            cnt.Add("<g text-anchor=\"end\" stroke-width=\"1\" fill=\"grey\" font-size=\"12px\">");
            for (int i = 0; i < marks.Length; i++)
            {
                var m = marks[i];
                var y = top + (marks[^1] - m) * vper + 4;
                cnt.Add("<text x=\"");
                cnt.Add(left - margin);
                cnt.Add("\" y=\"");
                cnt.Add(y);
                cnt.Add("\">");
                var intm = (int) m;
                if (intm == m)
                {
                    cnt.Add(intm);
                }
                else
                {
                    cnt.Add(m);
                }
                cnt.Add("</text>");
            }
            cnt.Add("</g>");

            // horitontal lines
            cnt.Add("<g stroke=\"silver\" stroke-width=\"1\" fill=\"transparent\">");
            for (int i = 0; i < marks.Length; i++)
            {
                var m = marks[i];
                var y = top + (marks[^1] - m) * vper;
                cnt.Add("<line x1=\"");
                cnt.Add(left);
                cnt.Add("\" y1=\"");
                cnt.Add(y);
                cnt.Add("\" x2=\"");
                cnt.Add(right);
                cnt.Add("\" y2=\"");
                cnt.Add(y);
                cnt.Add("\"/>");
            }
            cnt.Add("</g>");
        }

        public void WriteLegend<C>(C cnt) where C : DynamicContent, ISink
        {
            var xper = (maxx - margin * 2) / COLORS.Length;
            var y = maxy - margin - legendhei;
            cnt.Add("<g stroke-width=\"2\">");
            for (int i = 0; i < sets.Length; i++)
            {
                cnt.Add("<line x1=\"");
                cnt.Add(margin + i * xper);
                cnt.Add("\" y1=\"");
                cnt.Add(y);
                cnt.Add("\" x2=\"");
                cnt.Add(margin + i * xper + xper - 4);
                cnt.Add("\" y2=\"");
                cnt.Add(y);
                cnt.Add("\" stroke=\"");
                cnt.Add(COLORS[i]);
                cnt.Add("\"/>");
            }
            cnt.Add("</g>");

            cnt.Add("<g stroke-width=\"1\" fill=\"black\" font-size=\"10px\">");
            for (int i = 0; i < sets.Length; i++)
            {
                var txt = sets[i];
                cnt.Add("<text x=\"");
                cnt.Add(margin + i * xper);
                cnt.Add("\" y=\"");
                cnt.Add(y + 12);
                cnt.Add("\">");
                cnt.Add(txt);
                cnt.Add("</text>");
            }
            cnt.Add("</g>");
        }

        public IContent Dump()
        {
            throw new NotImplementedException();
        }
    }
}