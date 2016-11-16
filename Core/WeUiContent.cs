using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    ///
    /// <summary>
    /// For dynamical HTML5 content generation Tooled with WeUI
    /// </summary>
    ///
    public class WeUiContent : HtmlContent, IMenu, ISelectOptions
    {
        const int InitialCapacity = 8 * 1024;

        const string SM = "sm", MD = "md", LG = "lg", XL = "xl";

        const sbyte TableThs = 1, TableTrs = 2, FormFields = 3;

        sbyte ctx;


        public WeUiContent(bool raw, bool pooled, int capacity = InitialCapacity) : base(raw, pooled, capacity)
        {
        }

        public override string Type => "text/html; charset=utf-8";


        public Dictionary<string, string> Map { get; set; }

    }
}