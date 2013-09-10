using System;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Formatting;
using FubuCore.Reflection;
using FubuObjectBlocks.Writing;

namespace FubuObjectBlocks
{
    public class ObjectBlockWriter : IObjectBlockWriter
    {
        private readonly ITypeDescriptorCache _cache;
        private readonly IServiceLocator _services;
        private readonly IBlockWriterLibrary _writerLibrary;
        private readonly BlockRegistry _blocks;

        public ObjectBlockWriter(ITypeDescriptorCache cache, IServiceLocator services, IBlockWriterLibrary writerLibrary, BlockRegistry blocks)
        {
            _cache = cache;
            _services = services;
            _writerLibrary = writerLibrary;
            _blocks = blocks;
        }

        public string Write(object input)
        {
            var block = BlockFor(input);
            block.Sort(_blocks.Sorter);
            return block.ToString();
        }

        public ObjectBlock BlockFor(object input, string objectName = null)
        {
            var context = new BlockWritingContext(_services, this, _blocks, input);
            return BlockFor(input, context, objectName);
        }

        public ObjectBlock BlockFor(object input, BlockWritingContext context, string objectName = null)
        {
            Accessor implicitAccessor = null;
            var type = input.GetType();

            if (context.Accessor != null)
            {
                var parentSettings = _blocks.SettingsFor(context.Accessor.OwnerType);
                implicitAccessor = parentSettings.FindImplicitValue(type);
            }

            var implicitValue = implicitAccessor != null
                ? implicitAccessor.GetValue(input).ToString()
                : null;

            var properties = _cache.GetPropertiesFor(type).Values;

            return new ObjectBlock
            {
                Blocks = properties
                    .Where(x => x.GetValue(input, null) != null && !isImplicitValue(x, implicitAccessor))
                    .Select(x =>
                    {
                        context.StartProperty(x);

                        var writer = _writerLibrary.WriterFor(context);
                        var block = writer.Write(context);

                        context.FinishProperty();

                        return block;

                    }).ToList(),
                Name = _blocks.NameFor(new BlockToken(objectName)),
                ImplicitValue = implicitValue
            };
        }

        private static bool isImplicitValue(PropertyInfo property, Accessor implicitAccessor)
        {
            if (implicitAccessor == null) return false;
            return new SingleProperty(property).Equals(implicitAccessor);
        }

        public static ObjectBlockWriter Basic()
        {
            return Basic(BlockRegistry.Basic());
        }

        public static ObjectBlockWriter Basic(Action<BlockRegistry> configure)
        {
            var registry = BlockRegistry.Basic();
            configure(registry);

            return Basic(registry);
        }

        public static ObjectBlockWriter Basic(BlockRegistry registry)
        {
            var services = new InMemoryServiceLocator();
            services.Add<IDisplayFormatter>(new DisplayFormatter(services, new Stringifier()));

            return new ObjectBlockWriter(new TypeDescriptorCache(), services, BlockWriterLibrary.Basic(), registry);
        }
    }
}