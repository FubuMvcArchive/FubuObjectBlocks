using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuCore.Configuration;

namespace FubuObjectBlocks
{
    public class ObjectBlockValues : IValueSource
    {
        private readonly Type _type;
        private readonly ObjectBlock _root;
        private readonly IObjectBlockSettings _settings;
        
        public ObjectBlockValues(ObjectBlock root, Type type)
            : this(root, new ObjectBlockSettings(), type)
        {
        }

        public ObjectBlockValues(ObjectBlock root, IObjectBlockSettings settings, Type type)
        {
            _root = root;
            _settings = settings;
            _type = type;
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

            return _root.FindBlock<ObjectBlock>(key) != null;
        }

        public IValueSource GetChild(string key)
        {
            if (!HasChild(key))
            {
                return new SettingsData();
            }

            var child = _root.FindBlock<ObjectBlock>(key);
            return new ObjectBlockValues(child, _settings, _type);
        }

        public IEnumerable<IValueSource> GetChildren(string key)
        {
            key = _settings.Collection(_type, key);

            if (!Has(key)) return Enumerable.Empty<IValueSource>();

            var collectionType = _settings.FindCollectionType(_type, key);
            return _root
                .FindBlock<CollectionBlock>(key)
                .Blocks
                .Select(x => new ObjectBlockValues(x, _settings, collectionType))
                .ToList();
        }

        public void WriteReport(IValueReport report)
        {
            // no-op
        }

        public bool Value(string key, Action<BindingValue> callback)
        {
            //TODO: remove half of this check, can probably just right from _root.ImplicitValue if its set
            var implicitValue = _settings.ImplicitValue(_type, key);
            string value;
            if (implicitValue != null && _root.ImplicitValue != null)
            {
                value = _root.ImplicitValue;
            }
            else
            {
                if (!Has(key)) return false;
                value = Get(key) as string;
            }

            callback(new BindingValue
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