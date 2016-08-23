
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JsonText : IReader, IWriter
    {
	    // for input
	    private string text;

	    // for output
	    private char[] buffer;

	    private int pos;

	    public JsonText(string str)
	    {

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

	    public bool Read<T>(string name, ref T value) where T : ISerial
	    {
		    throw new System.NotImplementedException();
	    }

	    public bool Read(string name, ref List<string> value)
	    {
		    throw new System.NotImplementedException();
	    }

	    public bool Read(string name, ref string[] value)
	    {
		    throw new System.NotImplementedException();
	    }

	    public bool Read<T>(string name, ref List<T> value) where T : ISerial
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

	    public void Write(string name, ISerial value)
	    {
		    throw new System.NotImplementedException();
	    }

	    public bool Read(string name, ref bool value)
	    {
		    throw new System.NotImplementedException();
	    }

	    public bool Read<K, V>(string name, ref Dictionary<K, V> value)
	    {
		    throw new System.NotImplementedException();
	    }

	    public void Write(string name, bool value)
	    {
		    throw new System.NotImplementedException();
	    }

	    public void Write<T>(string name, List<T> list)
	    {
		    throw new System.NotImplementedException();
	    }

	    public void Write<V>(string name, Dictionary<string, V> dict)
	    {
		    throw new System.NotImplementedException();
	    }

	    public void Write(string name, params string[] array)
	    {
		    throw new System.NotImplementedException();
	    }

	    public void Write(string name, params ISerial[] array)
	    {
		    throw new System.NotImplementedException();
	    }
    }
}