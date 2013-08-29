using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuCore.Configuration;

namespace FubuObjectBlocks
{
    public class ObjectBlockValues<T> : IValueSource
    {
        private readonly ObjectBlock _root;
        private readonly IObjectBlockSettings _settings;
        
        public ObjectBlockValues(ObjectBlock root)
            : this(root, new ObjectBlockSettings())
        {
        }

        public ObjectBlockValues(ObjectBlock root, IObjectBlockSettings settings)
        {
            _root = root;
            _settings = settings;
        }

        public bool Has(string key)
        {
            return _root.Has(key);
        }

        public object Get(string key)
        {
            var property = _root.FindBlock<PropertyBlock>(key);
            if (property == null) return null;

            return property.Value;
        }

        public bool HasChild(string key)
        {
            if (!Has(key)) return false;

            return _root.FindBlock<ObjectBlock>(key).Blocks.Any();
        }

        public IValueSource GetChild(string key)
        {
            if (!HasChild(key))
            {
                return new SettingsData();
            }

            var child = _root.FindBlock<ObjectBlock>(key);
            return new ObjectBlockValues<T>(child, _settings);
        }

        public IEnumerable<IValueSource> GetChildren(string key)
        {
            key = _settings.Collection(typeof(T), key);

            if (!Has(key)) return Enumerable.Empty<IValueSource>();

            var collectionType = _settings.FindCollectionType(typeof (T), key);
            var builder = typeof (ObjectValueBuilder<>).CloseAndBuildAs<IObjectValueBuilder>(collectionType);
            return _root.FindBlock<CollectionItemBlock>(key).Blocks.Select(x => builder.Build(x, _settings)).ToList();
        }

        public void WriteReport(IValueReport report)
        {
            // no-op
        }

        public bool Value(string key, Action<BindingValue> callback)
        {
            var implicitValue = _settings.ImplicitValue(typeof(T), key);
            string value;
            if (implicitValue != null)
            {
                value = _root.Value;
            }
            else
            {
                if (!Has(key)) return false;
                value = Get(key) as string;
            }

            

            callback(new BindingValue()
            {
                RawKey = key,
                RawValue = value,
                Source = Provenance
            });

            return true;
        }

        public string Provenance { get { return "Fubu Object"; } }
    }
}