
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JsonText : IDocReader, IDocWriter
    {
	    // for input
	    private string _text;

	    // for output
	    private char[] _buffer;

	    private int _pos;

	    public bool ReadStart()
	    {
		    throw new System.NotImplementedException();
	    }

	    public bool ReadEnd()
	    {
		    throw new System.NotImplementedException();
	    }

	    public bool Read(string name, ref int value)
	    {
		    throw new System.NotImplementedException();
	    }

	    public bool Read(string name, ref decimal value)
	    {
		    throw new System.NotImplementedException();
	    }

	    public bool Read(string name, ref string value)
	    {
		    throw new System.NotImplementedException();
	    }

	    public bool Read<T>(string name, ref List<T> value) where T : IDoc
	    {
		    throw new System.NotImplementedException();
	    }

	    public void WriteStart()
	    {
		    throw new System.NotImplementedException();
	    }

	    public void WriteEnd()
	    {
		    throw new System.NotImplementedException();
	    }

	    public void Write(string name, int value)
	    {
		    throw new System.NotImplementedException();
	    }

	    public void Write(string name, decimal value)
	    {
		    throw new System.NotImplementedException();
	    }

	    public void Write(string name, string value)
	    {
		    throw new System.NotImplementedException();
	    }

	    public void Write<T>(string name, List<T> value) where T : IDoc
	    {
		    throw new System.NotImplementedException();
	    }
    }
}