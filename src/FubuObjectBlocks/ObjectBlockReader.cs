using System;
using FubuCore;
using FubuCore.Binding;

namespace FubuObjectBlocks
{
    public class ObjectBlockReader : IObjectBlockReader
    {
        private readonly IObjectBlockParser _parser;
        private readonly IObjectResolver _resolver;
        private readonly BlockRegistry _blocks;

        public ObjectBlockReader(IObjectBlockParser parser, IObjectResolver resolver, BlockRegistry blocks)
        {
            _parser = parser;
            _resolver = resolver;
            _blocks = blocks;
        }

        public T Read<T>(string input)
        {
            var settings = _blocks.SettingsFor(typeof (T));
            var block = _parser.Parse(input, settings);
            var result = _resolver.BindModel(typeof(T), new ObjectBlockValues<T>(block, settings));

            return result.Value.As<T>();
        }

        public ObjectBlock Read(string input)
        {
            return _parser.Parse(input, new ObjectBlockSettings());
        }

        public static ObjectBlockReader Basic()
        {
            return Basic(BlockRegistry.Basic());
        }

        public static ObjectBlockReader Basic(Action<BlockRegistry> configure)
        {
            var registry = BlockRegistry.Basic();
            configure(registry);

            return Basic(registry);
        }

        public static ObjectBlockReader Basic(BlockRegistry registry)
        {
            return new ObjectBlockReader(new ObjectBlockParser(), ObjectResolver.Basic(), registry);
        }
    }
}