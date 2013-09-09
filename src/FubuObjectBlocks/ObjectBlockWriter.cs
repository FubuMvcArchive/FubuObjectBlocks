using System.Linq;
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
            return Write(input, new ObjectBlockSettings());
        }

        public string Write<T, TMap>(T input) where TMap : ObjectBlockSettings<T>, new()
        {
            return Write(input, new TMap());
        }

        public string Write(object input, IObjectBlockSettings settings)
        {
            var block = BlockFor(input);
            return block.ToString();
        }

        public ObjectBlock BlockFor(object input, string objectName = null)
        {
            var type = input.GetType();
            var settings = _blocks.SettingsFor(type);
            var implicitAccessor = settings.FindImplicitValue(type);

            var implicitValue = implicitAccessor != null
                ? implicitAccessor.GetValue(input).ToString()
                : null;

            var properties = _cache.GetPropertiesFor(type).Values;

            return new ObjectBlock
            {
                Blocks = properties
                    .Where(x => x.GetValue(input, null) != null)
                    .Select(x =>
                    {
                        var name = new BlockName(new SingleProperty(x));
                        var context = new BlockWritingContext(_services, this, _blocks, name, input);
                        var writer = _writerLibrary.WriterFor(context);

                        return writer.Write(context);
                    }).ToList(),
                Name = _blocks.NameFor(new BlockName(objectName)),
                ImplicitValue = implicitValue
            };
        }


        public static ObjectBlockWriter Basic()
        {
            var services = new InMemoryServiceLocator();
            services.Add<IDisplayFormatter>(new DisplayFormatter(services, new Stringifier()));

            return new ObjectBlockWriter(new TypeDescriptorCache(), services, BlockWriterLibrary.Basic(), BlockRegistry.Basic());
        }
    }
}