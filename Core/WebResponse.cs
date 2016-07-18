using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
  public struct WebResponse : IDataOutput
  {
    HttpResponse _impl;

    private byte[] _buffer;

    private int _offset;

    private int _count;

    internal WebResponse(HttpResponse impl)
    {
      _impl = impl;
      _buffer = null;
      _offset = 0;
      _count = 0;
    }

    public void SetBody(byte[] buffer, int offset, int count)
    {
      _buffer = buffer;
      _offset = offset;
      _count = count;
      _impl.ContentLength = count;
    }


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