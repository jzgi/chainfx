﻿
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

	    public bool ReadArrayStart()
	    {
		    throw new System.NotImplementedException();
	    }

	    public bool ReadArrayEnd()
	    {
		    throw new System.NotImplementedException();
	    }

	    public bool ReadSep()
	    {
		    throw new System.NotImplementedException();
	    }

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

	    public bool Read<T>(string name, ref T value) where T : ISerial
	    {
		    throw new System.NotImplementedException();
	    }

	    public bool Read<T>(string name, ref List<T> value) where T : ISerial
	    {
		    throw new System.NotImplementedException();
	    }

	    public void WriteArrayStart()
	    {
		    throw new System.NotImplementedException();
	    }

	    public void WriteArrayEnd()
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

	    public void WriteSep()
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

	    public void Write<T>(string name, T value) where T : ISerial
	    {
		    throw new System.NotImplementedException();
	    }

	    public void Write<T>(string name, List<T> value) where T : ISerial
	    {
		    throw new System.NotImplementedException();
	    }
    }
}