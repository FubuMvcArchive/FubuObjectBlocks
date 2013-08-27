using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;

namespace FubuObjectBlocks
{
    public class ObjectBlockSettings : IObjectBlockSettings
    {
        private readonly IList<Type> _filledTypes = new List<Type>(); 
        private readonly IList<CollectionConfiguration> _collections = new List<CollectionConfiguration>();
        private static readonly TypeDescriptorCache Cache;

        static ObjectBlockSettings()
        {
            Cache = new TypeDescriptorCache();
        }


        protected void addCollection(CollectionConfiguration collection)
        {
            _collections.Add(collection);
        }

        private void fillSettings(Type type)
        {
            if (_filledTypes.Contains(type))
            {
                return;
            }

            Cache.ForEachProperty(type, property =>
            {
                var accessor = new SingleProperty(property);
                if (property.HasAttribute<BlockSettingsAttribute>())
                {
                    var settings = property.GetAttribute<BlockSettingsAttribute>();
                    _collections.Add(settings.ToConfiguration(type, property));
                }
                else if(!_collections.Any(x => x.Accessor.Equals(accessor)))
                {
                    var settings = new CollectionConfiguration(accessor)
                    {
                        Name = accessor.Name
                    };

                    _collections.Add(settings);
                }
            });

            _filledTypes.Add(type);
        }

        public string Collection(Type type, string key)
        {
            fillSettings(type);

            var map = _collections.SingleOrDefault(x => x.Accessor.Name.EqualsIgnoreCase(key));
            if (map != null)
            {
                return map.Name;
            }

            return key;
        }

        public Accessor ImplicitValue(Type type, ObjectBlock block, string key)
        {
            fillSettings(type);

            var map = _collections
                .SingleOrDefault(x => x.Implicit != null && x.Implicit.Name.EqualsIgnoreCase(key) && x.Implicit.OwnerType == type);
            return map == null ? null : map.Implicit;
        }

        public Accessor FindImplicitValue(Type type, ObjectBlock block)
        {
            fillSettings(type);

            var map = _collections.FirstOrDefault(x => x.Implicit != null && x.Implicit.OwnerType == type);
            return map == null ? null : map.Implicit;
        }

        public Type FindCollectionType(Type type, string key)
        {
            fillSettings(type);

            var collectionMap = _collections.SingleOrDefault(x => x.Name.EqualsIgnoreCase(key));
            if (collectionMap == null)
            {
                throw new InvalidOperationException("Could not find property: " + key);
            }

            return collectionMap.Accessor.PropertyType.FindInterfaceThatCloses(typeof(IEnumerable<>)).GetGenericArguments()[0];
        }
    }

    public class ObjectBlockSettings<T> : ObjectBlockSettings
    {
        public ConfigureCollectionExpression<TTarget> Collection<TTarget>(Expression<Func<T, IEnumerable<TTarget>>> expression)
        {
            var config = new CollectionConfiguration(ReflectionHelper.GetAccessor(expression));
            addCollection(config);

            return new ConfigureCollectionExpression<TTarget>(config);
        }

        public class ConfigureCollectionExpression<TTarget>
        {
            private readonly CollectionConfiguration _configuration;

            public ConfigureCollectionExpression(CollectionConfiguration configuration)
            {
                _configuration = configuration;
            }

            public ConfigureCollectionExpression<TTarget> ExpressAs(string name)
            {
                _configuration.Name = name;
                return this;
            }

            public ConfigureCollectionExpression<TTarget> ImplicitValue(Expression<Func<TTarget, object>> expression)
            {
                _configuration.Implicit = expression.ToAccessor();
                return this;
            }
        }
    }
}