using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuObjectBlocks
{
    public class CollectionItemBlock : IBlock
    {
        private readonly IList<PropertyBlock> _blocks = new List<PropertyBlock>();

        public CollectionItemBlock(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public IEnumerable<PropertyBlock> Blocks { get { return _blocks; } }

        public void Clear()
        {
            _blocks.Clear();
        }

        public void AddBlock(PropertyBlock block)
        {
            _blocks.Add(block);
        }

        public override string ToString()
        {
            return ToString(0);
        }

        public string ToString(int indent = 0)
        {
            return "";
        }
    }

    public class PropertyBlock : IBlock
    {
        public PropertyBlock(string name, string value)
        {
            Name = name;
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return ToString(0);
        }

        public string ToString(int indent)
        {
            return BlockIndenter.Indent("{0}: '{1}'".ToFormat(Name, Value), indent);
        }
    }
}