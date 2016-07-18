using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
  public struct WebResponse : IDataOutput
  {
    HttpResponse _impl;


    public void PutStart()
    {
      throw new System.NotImplementedException();
    }

    public void PutEnd()
    {
      throw new System.NotImplementedException();
    }

    public void Put(string name, int value)
    {
      throw new System.NotImplementedException();
    }

    public void Put(string name, decimal value)
    {
      throw new System.NotImplementedException();
    }

    public void Put(string name, string value)
    {
      throw new System.NotImplementedException();
    }

    public void Put<T>(string name, List<T> value) where T : IData
    {
      throw new System.NotImplementedException();
    }
  }
}