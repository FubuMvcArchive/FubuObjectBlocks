﻿using System;
using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuObjectBlocks
{
    public interface IObjectBlockSettings
    {
        string Collection(Type type, string key);
        
        Accessor ImplicitValue(Type type, string key);
        Accessor FindImplicitValue(Type type);

        Type FindCollectionType(Type type, string key);
        IEnumerable<string> KnownCollectionNames();
    }
}