using System.Collections.Generic;
using System.Linq;

namespace FubuObjectBlocks.Formatting
{
    public interface IBlockSorter
    {
        IEnumerable<IBlock> Sort(IEnumerable<IBlock> blocks);
    }

    public class BlockSorter : IBlockSorter
    {
        public IEnumerable<IBlock> Sort(IEnumerable<IBlock> blocks)
        {
            foreach (var propertyBlock in blocks.OfType<PropertyBlock>().OrderBy(x => x.Name))
            {
                yield return propertyBlock;
            }

            foreach (var nestedBlock in blocks.OfType<ObjectBlock>().OrderBy(x => x.Name))
            {
                nestedBlock.Sort(this);
                yield return nestedBlock;
            }

            foreach (var collectionBlock in blocks.OfType<CollectionBlock>().OrderBy(x => x.Name))
            {
                yield return collectionBlock;
            }
        }
    }
}