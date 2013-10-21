using System.Collections.Generic;
using System.Linq;

namespace FubuObjectBlocks.Formatting
{
    public class BlockSorter : IBlockSorter
    {
        public IEnumerable<IBlock> Sort(IEnumerable<IBlock> blocks)
        {
            var sorted = new List<IBlock>();

            sorted.AddRange(properties(blocks));
            sorted.AddRange(nested(blocks));
            sorted.AddRange(collections(blocks));

            return sorted;
        }

        private IEnumerable<IBlock> properties(IEnumerable<IBlock> blocks)
        {
            return blocks.OfType<PropertyBlock>().OrderBy(x => x.Name);
        }

        private IEnumerable<IBlock> nested(IEnumerable<IBlock> blocks)
        {
            var nested = new List<IBlock>();
            foreach (var nestedBlock in blocks.OfType<ObjectBlock>().OrderBy(x => x.Name))
            {
                nestedBlock.Sort(this);
                nested.Add(nestedBlock);
            }

            if (nested.Any())
            {
                nested.Add(new BlockSeparator());
            }

            return nested;
        }

        private IEnumerable<IBlock> collections(IEnumerable<IBlock> blocks)
        {
            var collections = new List<IBlock>();
            foreach (var block in blocks.OfType<CollectionBlock>().OrderBy(x => x.Name))
            {
                var last = collections.LastOrDefault();
                if (last != null && last.Name != block.Name)
                {
                    collections.Add(new BlockSeparator());
                }

                collections.Add(block);
            }

            return collections;
        }
    }
}