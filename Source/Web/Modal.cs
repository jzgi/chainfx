namespace ChainFX.Web
{
    public enum Modal
    {
        Anchor = 0x1000,

        /// <summary>
        /// To execute a script that calls the procedure asynchronously.
        /// </summary>
        AnchorScript = 0x1001,

        AnchorConfirm = 0x1002,

        AnchorCrop = 0x1004,

        /// <summary>
        /// To prompt with a dialog for gathering additional data to continue the location switch.
        /// </summary>
        AnchorPrompt = 0x1008,

        AnchorShow = 0x1010,

        /// <summary>
        /// To open a free-style dialog without the OK button, where the procedure can be called a number of times.
        /// </summary>
        AnchorOpen = 0x1020,

        AnchorStack = 0x1040,


        Button = 0x2000,

        ButtonPick = 0x2100,

        ButtonScript = 0x2001,

        ButtonPickScript = 0x2101,

        ButtonConfirm = 0x2002,

        ButtonPickConfirm = 0x2102,

        ButtonCrop = 0x2104,

        ButtonPickCrop = 0x2104,

        /// <summary>
        /// To show a dialog for gathering additional data to continue the button submission for a post action.
        /// </summary>
        ButtonPrompt = 0x2008,

        ButtonPickPrompt = 0x2108,

        ButtonShow = 0x2010,

        ButtonPickShow = 0x2110,

        /// <summary>
        /// To open a free-style dialog, passing current form context
        /// </summary>
        ButtonOpen = 0x2020,

        ButtonPickOpen = 0x2120,

        ButtonAStack = 0x2040,

        ButtonPickAStack = 0x2140,
    }
}