using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Greatbone.Core
{
  ///
  /// Represents a (sub)controller that consists of a group of action methods, as well as a folder of static files.
  ///
  public abstract class WebSub : WebController
  {
    // the collection of actions declared by this sub-controller
    readonly Set<WebAction> _actions = new Set<WebAction>(32);

    // the default action
    readonly WebAction _defaction;


    public Checker Checker { get; internal set; }

    // the argument makes state-passing more convenient
    public WebSub(WebCreationContext wcc) : base(wcc)
    {
      Type type = GetType();

      // introspect action methods
      foreach (MethodInfo mi in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
      {
        ParameterInfo[] pis = mi.GetParameters();
        if (pis.Length == 1 && pis[0].ParameterType == typeof(WebContext))
        {
          WebAction a = new WebAction(this, mi);
          if (a.Key.Equals("Default"))
          {
            _defaction = a;
          }
          _actions.Add(a);
        }
      }
    }


    public WebAction GetAction(String action)
    {
      if (string.IsNullOrEmpty(action))
      {
        return _defaction;
      }
      return _actions[action];
    }

    public virtual void Handle(string relative, WebContext wc)
    {
      // static handling
      if (Statics != null && relative.IndexOf('.') != -1)
      {
        Static sta;
        if (Statics.TryGet(relative, out sta))
        {
          wc.Response.SetContent("", sta.Content, 0, sta.Length);
        }
        return;
      }

      // action handling
      WebAction a;
      if (_actions.TryGet(relative, out a))
      {
        a.Do(wc);
      }
      else
      {
        // send not found
      }
    }

    public virtual void Default(WebContext wc)
    {
      if (DefaultStatic != null)
      {
      }
      else
      {
        // send not implemented
      }
    }
  }


  ///
  /// Represents a multiplexed sub-controller that consists of a group of action methods.
  ///
  public abstract class WebSub<TZone> : WebController where TZone : IZone
  {
    // the collection of multiplexed actions declared by this sub-controller
    readonly Set<WebAction<TZone>> _actions = new Set<WebAction<TZone>>(32);

    // the default action
    readonly WebAction<TZone> _defaction;

    // the argument makes state-passing more convenient
    protected WebSub(WebCreationContext wcc) : base(wcc)
    {
      Type type = GetType();

      // introspect action methods
      foreach (MethodInfo mi in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
      {
        ParameterInfo[] pis = mi.GetParameters();
        if (pis.Length == 2 &&
            pis[0].ParameterType == typeof(WebContext) &&
            pis[0].ParameterType == typeof(TZone))
        {
          WebAction<TZone> a = new WebAction<TZone>(this, mi);
          if (a.Key.Equals("Default"))
          {
            _defaction = a;
          }
          _actions.Add(a);
        }
      }
    }


    public Checker<TZone> Checker { get; internal set; }

    public WebAction<TZone> GetAction(string action)
    {
      return _actions[action];
    }

    public virtual void Handle(string relative, WebContext wc)
    {
      // static handling
      if (Statics != null && relative.IndexOf('.') != -1)
      {
        Static sta;
        if (Statics.TryGet(relative, out sta))
        {
          wc.Response.SetContent("", sta.Content, 0, sta.Length);
        }
        return;
      }

      // action handling
      WebAction<TZone> a;
      if (_actions.TryGet(relative, out a))
      {
        a.Do(wc, (TZone) (wc.Zone));
      }
      else
      {
        // send not found
      }
    }

    public virtual void Default(WebContext wc, TZone zone)
    {
      if (DefaultStatic != null)
      {
        wc.Response.SetContent(DefaultStatic.ContentType, DefaultStatic.Content, 0, DefaultStatic.Length);
      }
      else
      {
        // send not implemented
        wc.Response.StatusCode = 404;
      }
    }
  }
}