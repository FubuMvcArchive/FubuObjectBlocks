using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;

namespace FubuObjectBlocks
{
    public static class BlockExtensions
    {
        public static bool IsCollectionBlockCompatible(this Type type)
        {
            //REMOVE: replaced with IsGenericEnumerable()
            return type.Closes(typeof (IEnumerable<>));
        }

        public static CollectionConfiguration ToCollectionConfiguration(this Accessor accessor)
        {
            var settings = accessor.InnerProperty.GetAttribute<BlockSettingsAttribute>();
            return settings.ToConfiguration(accessor.InnerProperty);
        }

        public static string FirstLetterLowercase(this string name)
        {
            return name.Substring(0, 1).ToLower() + name.Substring(1);
        }
    }

    public class ObjectBlockSettings : IObjectBlockSettings
    {
        private static readonly TypeDescriptorCache Cache;
        private static readonly Cache<Accessor, CollectionConfiguration> ByAccessor;
        private static readonly Cache<Type, IList<CollectionConfiguration>> CollectionsFor;

        static ObjectBlockSettings()
        {
            Cache = new TypeDescriptorCache();

            ByAccessor = new Cache<Accessor, CollectionConfiguration>(
                x => x.InnerProperty.HasAttribute<BlockSettingsAttribute>()
                    ? x.ToCollectionConfiguration()
                    : new CollectionConfiguration(x));

            CollectionsFor = new Cache<Type, IList<CollectionConfiguration>>(
                type => Cache.GetPropertiesFor(type)
                    .Values
                    .Where(x => x.PropertyType.IsGenericEnumerable())
                    .Select(x => ByAccessor[new SingleProperty(x)])
                    .ToList());
        }

        public static CollectionConfiguration Register(Accessor accessor)
        {
            var configuration = new CollectionConfiguration(accessor);
            ByAccessor[accessor] = configuration;
            return configuration;
        }

        public string Collection(Type type, string key)
        {
            var map = CollectionsFor[type].SingleOrDefault(x => x.Accessor.Name.EqualsIgnoreCase(key));
            return map != null ? map.Name : key;
        }

        public Accessor ImplicitValue(Type type, string key)
        {
            return ImplicitFor(type, x => x.Implicit.Name.EqualsIgnoreCase(key));
        }

        public CollectionConfiguration ImplicitCollectionFor(Type type, Func<CollectionConfiguration,bool> extraCondition = null)
        {
            extraCondition = extraCondition ?? (x => true);
            return Implicits().FirstOrDefault(x => x.Implicit.OwnerType == type && extraCondition(x));
        }

        public Accessor ImplicitFor(Type type, Func<CollectionConfiguration,bool> extraCondition = null)
        {
            extraCondition = extraCondition ?? (x => true);
            var map = ImplicitCollectionFor(type, extraCondition);
            return map != null ? map.Implicit : null;
        }

        public IEnumerable<CollectionConfiguration> Implicits()
        {
            return CollectionsFor.GetAllKeys()
                .SelectMany(x => CollectionsFor[x])
                .Where(x => x.Implicit != null);
        }

        public Accessor FindImplicitValue(Type type)
        {
            return ImplicitFor(type);
        }

        public Type FindCollectionType(Type type, string key)
        {
            var collectionMap = CollectionsFor[type].SingleOrDefault(x => x.Name.EqualsIgnoreCase(key));
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
            var config = Register(ReflectionHelper.GetAccessor(expression));
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