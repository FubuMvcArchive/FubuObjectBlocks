using System.Collections.Generic;

namespace FubuObjectBlocks.Formatting
{
    public interface IBlockSorter
    {
        IEnumerable<IBlock> Sort(IEnumerable<IBlock> blocks);
    }
}