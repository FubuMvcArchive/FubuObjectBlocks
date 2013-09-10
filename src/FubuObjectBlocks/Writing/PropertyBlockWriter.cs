using FubuCore;
using FubuCore.Reflection;

namespace FubuObjectBlocks.Writing
{
    public class PropertyBlockWriter : IBlockWriter
    {
        public bool Matches(BlockWritingContext context)
        {
            return context.MatchesAccessor(x => x.PropertyType.IsSimple() || x.PropertyType == typeof (decimal));
        }

        public IBlock Write(BlockWritingContext context)
        {
            var name = context.GetBlockName();
            var rawValue = context.RawValue;

            var value = context.Formatter.GetDisplayForValue(context.Accessor, rawValue);
            return new PropertyBlock(name)
            {
                Value = value
            };
        }
    }

    public class ImplicitValueBlockWriter : IBlockWriter
    {
        public bool Matches(BlockWritingContext context)
        {
            return context.MatchesAccessor(x => x.HasAttribute<ImplicitValueAttribute>());
        }

        public IBlock Write(BlockWritingContext context)
        {
            var name = context.GetBlockName();
            var rawValue = context.RawValue;

            var value = context.Formatter.GetDisplayForValue(context.Accessor, rawValue);
            return new PropertyBlock(name)
            {
                Value = value
            };
        }
    }
}