using System.Collections.Generic;
using System.Linq;

namespace FubuObjectBlocks
{
    public class BlockAccumulator
    {
        public int Rank { get; set; }

        private readonly Stack<ObjectBlock> _objectBlocks;

        public BlockAccumulator(ObjectBlock root)
        {
            _objectBlocks = new Stack<ObjectBlock>();
            _objectBlocks.Push(root);
        }

        public BlockAccumulator AddBlock(IBlock block)
        {
            CurrentObject.AddBlock(block);
            return this;
        }

        private ObjectBlock CurrentObject
        {
            get { return _objectBlocks.Peek(); }
        }

        public ObjectBlock Root
        {
            get { return _objectBlocks.Reverse().First(); }
        }

        public BlockAccumulator Nest(ObjectBlock block)
        {
            CurrentObject.AddBlock(block);
            _objectBlocks.Push(block);
            Rank = Rank + 1;
            return this;
        }

        public BlockAccumulator SetRank(int rank)
        {
            while (Rank > rank)
            {
                _objectBlocks.Pop();
                Rank = Rank - 1;
            }
            return this;
        }
    }
}