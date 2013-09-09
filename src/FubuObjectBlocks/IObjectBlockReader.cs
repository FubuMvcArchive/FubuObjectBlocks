namespace FubuObjectBlocks
{
    public interface IObjectBlockReader
    {
        T Read<T>(string input);
        ObjectBlock Read(string input);
    }
}