using System.Collections.Generic;
using System.Linq;

namespace FubuObjectBlocks
{
    public class CollectionItemBlock : IBlock
    {
        private IList<ObjectBlock> _blocks = new List<ObjectBlock>();

        public CollectionItemBlock(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public IList<ObjectBlock> Blocks
        {
            get { return _blocks; }
            set { _blocks = value; }
        }

        public void Clear()
        {
            _blocks.Clear();
        }

        public void AddBlock(ObjectBlock block)
        {
            _blocks.Add(block);
        }

        public override string ToString()
        {
            return ToString(0);
        }

        public string ToString(int indent = 0)
        {
            return Blocks
                .Select(x => x.OneLineSummary(indent))
                .Join("");
        }
    }
}