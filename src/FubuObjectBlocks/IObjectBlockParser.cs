namespace FubuObjectBlocks
{
    public interface IObjectBlockParser
    {
        ObjectBlock Parse(string input, IObjectBlockSettings settings);
    }
}