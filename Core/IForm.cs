namespace Greatbone.Core
{
    ///
    /// A data object that can be output as HTML form(s).
    ///
    public interface IForm : IData
    {
        void WriteForm(HtmlContent h, ushort proj = 0);
    }
}
