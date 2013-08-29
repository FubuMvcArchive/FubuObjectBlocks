using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuObjectBlocks
{
    public class ObjectBlock : IBlock
    {
        private IList<IBlock> _blocks = new List<IBlock>();

        public IList<IBlock> Blocks
        {
            get { return _blocks; }
            set { _blocks = value; }
        }

        public ObjectBlock()
        {
        }

        //TODO: make sure this gets set
        public ObjectBlock(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public void AddBlock(IBlock block)
        {
            _blocks.Add(block);
        }

        public bool Has(string name)
        {
            return FindBlock(name) != null;
        }

        public IBlock FindBlock(string name)
        {
            return _blocks.SingleOrDefault(x => x.Name.EqualsIgnoreCase(name));
        }

        public TBlock FindBlock<TBlock>(string name) where TBlock : IBlock
        {
            return _blocks.OfType<TBlock>().SingleOrDefault(x => x.Name.EqualsIgnoreCase(name));
        }

        public PropertyBlock FindProperty(string name)
        {
            return FindBlock<PropertyBlock>(name);
        }

        public ObjectBlock FindNested(string name)
        {
            return FindBlock<ObjectBlock>(name);
        }

        public string OneLineSummary(int indent = 0)
        {
            var nameAndValue = "{0} '{1}'".ToFormat(Name, Value);
            var content = new[] {nameAndValue}
                .Concat(Blocks.OfType<PropertyBlock>().Select(p => p.ToString()))
                .Join(", ");
            return "{0}{1}".ToFormat(BlockIndenter.Indent(content, indent), Environment.NewLine);
        }

        public string ToString(int indent = 0)
        {
            return new[] { BlockIndenter.Indent("{0}:".ToFormat(Name), indent) }
                .Concat(Blocks.Select(x => x.ToString(indent + 1)))
                .Join(Environment.NewLine);
        }
    }
}