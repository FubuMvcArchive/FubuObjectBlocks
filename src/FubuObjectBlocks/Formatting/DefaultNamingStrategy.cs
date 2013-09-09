namespace FubuObjectBlocks.Formatting
{
    public class DefaultBlockNamingStrategy : IBlockNamingStrategy
    {
        public bool Matches(BlockName name)
        {
            return !name.IsEmpty();
        }

        public string NameFor(BlockName name)
        {
            var blockName = name.Value;
            return blockName.Substring(0, 1).ToLower() + blockName.Substring(1);
        }
    }
}