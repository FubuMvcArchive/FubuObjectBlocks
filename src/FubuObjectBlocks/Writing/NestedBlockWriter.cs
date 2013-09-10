namespace FubuObjectBlocks.Writing
{
    public class NestedBlockWriter : IBlockWriter
    {
        public bool Matches(BlockWritingContext context)
        {
            return true;
        }

        public IBlock Write(BlockWritingContext context)
        {
            return context.Writer.BlockFor(context.RawValue, context.Token.Value);
        }
    }
}