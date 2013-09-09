using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuObjectBlocks.Writing
{
    public class CollectionBlockWriter : IBlockWriter
    {
        public bool Matches(BlockWritingContext context)
        {
            return context.MatchesAccessor(x => x.PropertyType.IsGenericEnumerable());
        }

        public IBlock Write(BlockWritingContext context)
        {
            var type = context.Name.Accessor.OwnerType;
            var settings = context.Registry.SettingsFor(type);
            var name = settings.Collection(type, context.Name.Value);

            var collectionName = context.Registry.NameFor(new BlockName(name));
            
            IEnumerable<object> items = new object[0];

            var rawValue = context.RawValue as IEnumerable;
            if (rawValue != null)
            {
                items = rawValue.Cast<object>();
            }

            return new CollectionBlock(collectionName)
            {
                Blocks = items
                    .Select(value => context.Writer.BlockFor(value, context.Name.Value))
                    .ToList()
            };
        }
    }
}