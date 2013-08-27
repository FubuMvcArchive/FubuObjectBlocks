using System;
using FubuCore.Reflection;

namespace FubuObjectBlocks
{
    public interface IObjectBlockSettings
    {
        string Collection(Type type, string key);
        
        Accessor ImplicitValue(Type type, ObjectBlock block, string key);
        Accessor FindImplicitValue(Type type, ObjectBlock block);

        Type FindCollectionType(Type type, string key);
    }
}