namespace FubuObjectBlocks
{
    public interface IObjectBlockSerializer
    {
        T Deserialize<T>(string input);
        T Deserialize<T, TMap>(string input) where TMap : ObjectBlockSettings<T>, new();

        string Serialize(object input);
        string Serialize(object input, IObjectBlockSettings settings);
        string Serialize<T, TMap>(T input) where TMap : ObjectBlockSettings<T>, new();
    }
}