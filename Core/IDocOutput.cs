﻿using System.Collections.Generic;

namespace Greatbone.Core
{
    ///
    /// DataReader/ParameterCollection, JSON or binary
    public interface IDocOutput
    {
        void PutStart();

        void PutEnd();

        void Put(string name, int value);

        void Put(string name, decimal value);

        void Put(string name, string value);

        void Put<T>(string name, List<T> value) where T : IDoc;
    }
}