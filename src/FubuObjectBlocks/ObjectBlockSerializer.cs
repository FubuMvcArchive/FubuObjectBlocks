using System;
using System.Collections;
using System.Linq;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Formatting;
using FubuCore.Reflection;

namespace FubuObjectBlocks
{
    public class ObjectBlockSerializer : IObjectBlockSerializer
    {
        private readonly IObjectBlockParser _parser;
        private readonly IObjectResolver _resolver;
        private readonly ITypeDescriptorCache _cache;
        private readonly IDisplayFormatter _displayFormatter;

        public ObjectBlockSerializer(IObjectBlockParser parser, IObjectResolver resolver, ITypeDescriptorCache cache, IDisplayFormatter displayFormatter)
        {
            _parser = parser;
            _resolver = resolver;
            _cache = cache;
            _displayFormatter = displayFormatter;
        }

        public T Deserialize<T>(string input)
        {
            var block = _parser.Parse(input);
            var result = _resolver.BindModel(typeof(T), new ObjectBlockValues<T>(block));

            return result.Value.As<T>();
        }

        public T Deserialize<T, TMap>(string input) where TMap : ObjectBlockSettings<T>, new()
        {
            var block = _parser.Parse(input);
            var result = _resolver.BindModel(typeof(T), new ObjectBlockValues<T>(block, new TMap()));

            return result.Value.As<T>();
        }

        public string Serialize(object input)
        {
            return Serialize(input, new ObjectBlockSettings());
        }

        public string Serialize<T, TMap>(T input) where TMap : ObjectBlockSettings<T>, new()
        {
            return Serialize(input, new TMap());
        }

        public string Serialize(object input, IObjectBlockSettings settings)
        {
            return BlockFor(input, settings).ToString();
        }

        public ObjectBlock BlockFor(object input, IObjectBlockSettings settings)
        {
            return MakeBlock(input, settings);
        }

        private ObjectBlock MakeBlock(object input, IObjectBlockSettings settings, string objectName = null)
        {
            var type = input.GetType();
            var implicitValue = settings.FindImplicitValue(type);
            var implicitValueBlock = implicitValue != null
                ? new PropertyBlock(implicitValue.Name)
                {
                    Value = _displayFormatter.GetDisplayForValue(implicitValue, implicitValue.GetValue(input))
                }
                : null;

            Func<string, string> formatName = x => x != null ? x.FirstLetterLowercase() : null;

            return new ObjectBlock
            {
                Blocks = _cache.GetPropertiesFor(type)
                    .Values
                    .Where(x => implicitValue == null || !implicitValue.Equals(new SingleProperty(x)))
                    .Select(x =>
                    {
                        //TODO: strategy pattern in here?
                        var name = formatName(x.Name);
                        var rawValue = x.GetValue(input, null);

                        //TODO: doesn't seem like ImplicitValue attribute has a place anymore
                        if (x.PropertyType.IsSimple() || x.HasAttribute<ImplicitValueAttribute>()/*TODO:remove this part if we don't need it*/)
                        {
                            var value = _displayFormatter.GetDisplayForValue(new SingleProperty(x), rawValue);
                            return new PropertyBlock(name)
                            {
                                Value = value
                            };
                        }

                        if (x.PropertyType.IsGenericEnumerable())
                        {
                            var collectionName = settings.Collection(type, name);
                            return new CollectionItemBlock(name)
                            {
                                Name = collectionName,
                                Blocks = (rawValue as IEnumerable)
                                    .Cast<object>()
                                    .Select(value => MakeBlock(value, settings, x.Name))
                                    .ToList()
                            };
                        }

                        return (IBlock) MakeBlock(rawValue, settings, x.Name);
                    }).ToList(),
                Name = formatName(objectName),
                ImplicitValue = implicitValueBlock
            };
        }

        public static ObjectBlockSerializer Basic()
        {
            var cache = new TypeDescriptorCache();
            var formatter = new DisplayFormatter(new InMemoryServiceLocator(), new Stringifier());
            return new ObjectBlockSerializer(new ObjectBlockParser(), ObjectResolver.Basic(), cache, formatter);
        }
    }
}