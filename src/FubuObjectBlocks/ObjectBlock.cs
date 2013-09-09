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

        public ObjectBlock(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public string ImplicitValue { get; set; }

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
            return GetBlocks<TBlock>().SingleOrDefault(x => x.Name.EqualsIgnoreCase(name));
        }

        public IEnumerable<TBlock> GetBlocks<TBlock>()
        {
            return _blocks.OfType<TBlock>();
        }

        public PropertyBlock FindProperty(string name)
        {
            return FindBlock<PropertyBlock>(name);
        }

        public ObjectBlock FindNested(string name)
        {
            return FindBlock<ObjectBlock>(name);
        }

        public CollectionBlock FindCollection(string name)
        {
            return FindBlock<CollectionBlock>(name);
        }

        public ObjectBlock MakeCollections(IObjectBlockSettings settings)
        {
            var collectionNames = settings.KnownCollectionNames().ToList();

            var items = Blocks
                .OfType<ObjectBlock>()
                .GroupBy(x => x.Name)
                //known collection names (handles singles) or any grouping of 2 or more (handles unknowns)
                .Where(x => collectionNames.Contains(x.Key) || x.Count() > 1)
                .ToDictionary(x => x.Key, x => x.ToList());

            var allItems = items.SelectMany(x => x.Value).ToList();
            var collections = items.Select(x => new CollectionBlock(x.Key) {Blocks = x.Value});

            _blocks.RemoveAll(allItems.Contains);
            _blocks.AddRange(collections);
            allItems.Each(x => x.MakeCollections(settings));
            return this;
        }

        public string OneLineSummary(string collectionName = null, int indent = 0)
        {
            //TODO: one line summary not valid if there is no Name
            var nameAndValue = "{0} '{1}'".ToFormat(collectionName ?? Name, ImplicitValue);
            var content = new[] {nameAndValue}
                .Concat(GetBlocks<PropertyBlock>().Select(p => p.ToString()))
                .Join(", ");
            return "{0}{1}".ToFormat(BlockIndenter.Indent(content, indent), Environment.NewLine);
        }

        public string ToString(int indent = 0)
        {
            var hasName = Name.IsNotEmpty();
            var objectTitle = hasName
                ? new[] {BlockIndenter.Indent("{0}:".ToFormat(Name), indent)}
                : new string[] {};
            var nextIndentAmount = hasName ? indent + 1 : indent;

            return objectTitle
                .Concat(Blocks.Select(x => x.ToString(nextIndentAmount)))
                .Join(Environment.NewLine);
        }
    }
}