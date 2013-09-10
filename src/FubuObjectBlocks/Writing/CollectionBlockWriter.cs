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
            var type = context.Token.Accessor.OwnerType;
            var settings = context.Registry.SettingsFor(type);
            var name = settings.Collection(type, context.Token.Value);

            var collectionName = context.Registry.NameFor(new BlockToken(name));
            
            IEnumerable<object> items = new object[0];

            var rawValue = context.RawValue as IEnumerable;
            if (rawValue != null)
            {
                items = rawValue.Cast<object>();
            }

            return new CollectionBlock(collectionName)
            {
                Blocks = items
                    .Select(value =>
                    {
                        context.StartObject(value);
                        var block = context.Writer.BlockFor(value, context, context.Token.Value);

                        context.FinishObject();

                        return block;
                    })
                    .ToList()
            };
        }
    }
}