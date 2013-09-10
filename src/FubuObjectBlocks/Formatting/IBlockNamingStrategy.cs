namespace FubuObjectBlocks.Formatting
{
    public interface IBlockNamingStrategy
    {
        bool Matches(BlockToken token);
        string NameFor(BlockToken blockToken);
    }
}