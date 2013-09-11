using System;

namespace FubuObjectBlocks
{
    public interface IObjectBlockReader
    {
        T Read<T>(string input);
        object Read(Type type, string input);
        object Read(Type type, ObjectBlock block);
        ObjectBlock Read(string input);
    }
}