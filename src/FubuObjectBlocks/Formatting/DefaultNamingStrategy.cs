namespace FubuObjectBlocks.Formatting
{
    public class DefaultBlockNamingStrategy : IBlockNamingStrategy
    {
        public bool Matches(BlockToken token)
        {
            return !token.IsEmpty();
        }

        public string NameFor(BlockToken token)
        {
            var blockName = token.Value;
            return blockName.Substring(0, 1).ToLower() + blockName.Substring(1);
        }
    }
}