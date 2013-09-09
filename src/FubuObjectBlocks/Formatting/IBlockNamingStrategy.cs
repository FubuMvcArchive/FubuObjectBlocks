namespace FubuObjectBlocks.Formatting
{
    public interface IBlockNamingStrategy
    {
        bool Matches(BlockName name);
        string NameFor(BlockName blockName);
    }
}