namespace FubuObjectBlocks.Writing
{
    public interface IBlockWriter
    {
        bool Matches(BlockWritingContext context);
        IBlock Write(BlockWritingContext context);
    }
}