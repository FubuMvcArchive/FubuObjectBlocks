namespace FubuObjectBlocks.Formatting
{
    public class EmptyBlockNamingStrategy : IBlockNamingStrategy
    {
        public bool Matches(BlockToken token)
        {
            return token.IsEmpty();
        }

        public string NameFor(BlockToken blockToken)
        {
            return null;
        }
    }
}