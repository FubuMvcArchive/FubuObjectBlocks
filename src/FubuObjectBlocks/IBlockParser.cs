namespace FubuObjectBlocks
{
    public interface IBlockParser
    {
        bool IsMatch(string input);
        IBlock Parse(string input);
        BlockAccumulator Include(BlockAccumulator acc, IBlock block);
    }
}