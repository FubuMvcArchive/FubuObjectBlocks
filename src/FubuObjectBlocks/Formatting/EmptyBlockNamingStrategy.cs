namespace FubuObjectBlocks.Formatting
{
    public class EmptyBlockNamingStrategy : IBlockNamingStrategy
    {
        public bool Matches(BlockName name)
        {
            return name.IsEmpty();
        }

        public string NameFor(BlockName blockName)
        {
            return null;
        }
    }
}