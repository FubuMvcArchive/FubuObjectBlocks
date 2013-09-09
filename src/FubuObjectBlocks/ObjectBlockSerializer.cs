using FubuCore;
using FubuCore.Binding;

namespace FubuObjectBlocks
{
    public class ObjectBlockSerializer : IObjectBlockSerializer
    {
        private readonly IObjectBlockParser _parser;
        private readonly IObjectResolver _resolver;

        public ObjectBlockSerializer(IObjectBlockParser parser, IObjectResolver resolver)
        {
            _parser = parser;
            _resolver = resolver;
        }

        public T Deserialize<T>(string input)
        {
            return Deserialize<T, ObjectBlockSettings<T>>(input);
        }

        public T Deserialize<T, TSettings>(string input) where TSettings : ObjectBlockSettings<T>, new()
        {
            var settings = new TSettings();
            settings.Include(typeof (T));
            var block = _parser.Parse(input, settings);
            var result = _resolver.BindModel(typeof (T), new ObjectBlockValues<T>(block, settings));

            return result.Value.As<T>();
        }

        public static ObjectBlockSerializer Basic()
        {
            return new ObjectBlockSerializer(new ObjectBlockParser(), ObjectResolver.Basic());
        }
    }
}