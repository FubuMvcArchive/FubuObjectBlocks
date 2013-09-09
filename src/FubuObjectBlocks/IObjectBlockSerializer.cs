namespace FubuObjectBlocks
{
    public interface IObjectBlockSerializer
    {
        T Deserialize<T>(string input);
        T Deserialize<T, TMap>(string input) where TMap : ObjectBlockSettings<T>, new();
    }
}