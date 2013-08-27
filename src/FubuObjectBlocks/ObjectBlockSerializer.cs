using System.Collections;
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
            var block = BlockFor(input, settings);
            return block.Write();
        }

        public ObjectBlock BlockFor(object input, IObjectBlockSettings settings)
        {
            var root = new ObjectBlock();

            fillBlock(root, input, settings);

            return root;
        }

        private void fillBlock(ObjectBlock block, object input, IObjectBlockSettings settings)
        {
            var type = input.GetType();
            var implicitValue = settings.FindImplicitValue(type, block);

            _cache.ForEachProperty(type, property =>
            {
                if (implicitValue != null && implicitValue.Equals(new SingleProperty(property)))
                {
                    return;
                }

                var valueBlock = new ObjectBlock();
                var propBlock = new PropertyBlock(toBlockPropertyName(property.Name)) { Block = valueBlock };

                if (property.PropertyType.IsSimple() || property.HasAttribute<ImplicitValueAttribute>())
                {
                    var rawValue = property.GetValue(input, null);
                    valueBlock.Value = _displayFormatter.GetDisplayForValue(new SingleProperty(property), rawValue);
                }
                else if (property.PropertyType.IsGenericEnumerable())
                {
                    var values = property.GetValue(input, null) as IEnumerable;
                    if (values != null)
                    {
                        propBlock.Clear();
                        propBlock.Name = settings.Collection(type, propBlock.Name);
                        foreach (var value in values)
                        {
                            var childBlock = new ObjectBlock();
                            fillBlock(childBlock, value, settings);

                            propBlock.AddBlock(childBlock);
                        }
                    }
                }
                else
                {
                    var child = property.GetValue(input, null);
                    fillBlock(valueBlock, child, settings);
                }

                block.AddProperty(propBlock);
            });

            
            if (implicitValue != null)
            {
                var value = implicitValue.GetValue(input);
                block.Value = _displayFormatter.GetDisplayForValue(implicitValue, value);
            }
        }

        private static string toBlockPropertyName(string name)
        {
            return name.Substring(0, 1).ToLower() + name.Substring(1);
        }

        public static ObjectBlockSerializer Basic()
        {
            var cache = new TypeDescriptorCache();
            var formatter = new DisplayFormatter(new InMemoryServiceLocator(), new Stringifier());
            return new ObjectBlockSerializer(new ObjectBlockParser(), ObjectResolver.Basic(), cache, formatter);
        }
    }
}