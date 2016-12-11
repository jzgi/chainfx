namespace Greatbone.Core
{
    public abstract class WebControl
    {
        WebService service;

        readonly CheckAttribute[] checks;

        // if requires auth through header or cookie
        readonly bool header, cookie;

        readonly FilterAttribute[] filters;

        readonly UiAttribute ui;


        internal bool Check(WebActionContext ac)
        {
            // access check 
            if (checks != null)
            {
                if (header && ac.Principal == null)
                {
                    ac.StatusCode = 401; // unauthorized
                    ac.SetHeader("WWW-Authenticate", "Bearer");
                    return false;
                }
                else if (cookie && ac.Principal == null)
                {
                    string loc = service.SignOn + "?orig=" + ac.Uri;
                    ac.SetHeader("Location", loc);
                    ac.StatusCode = 303; // see other - redirect to signon url
                    return false;
                }

                for (int i = 0; i < checks.Length; i++)
                {
                    if (!checks[i].Check(ac))
                    {
                        ac.StatusCode = 403; // forbidden
                        return false;
                    }
                }
            }
            return true;
        }

        internal void AlterBefore(WebActionContext ac)
        {
            if (filters == null) return;

            for (int i = 0; i < filters.Length; i++)
            {
                filters[i].Before(ac);
            }
        }

        internal void AlterAfter(WebActionContext ac)
        {
            if (filters == null) return;

            // execute in reversed order
            for (int i = filters.Length - 1; i <= 0; i--)
            {
                filters[i].After(ac);
            }
        }
    }
}