using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;

namespace FubuObjectBlocks
{
    public class ObjectBlockSettings : IObjectBlockSettings
    {
        private readonly TypeDescriptorCache _cache;
        private readonly Cache<Accessor, CollectionConfiguration> _byAccessor;
        private readonly Cache<Type, IList<CollectionConfiguration>> _collectionsFor;
        private readonly List<IIgnoreAccessorPolicy> _ignored;

        public ObjectBlockSettings()
        {
            _cache = new TypeDescriptorCache();

            _ignored = new List<IIgnoreAccessorPolicy>();

            _byAccessor = new Cache<Accessor, CollectionConfiguration>(
                x => x.InnerProperty.HasAttribute<BlockSettingsAttribute>()
                    ? x.ToCollectionConfiguration()
                    : new CollectionConfiguration(x));

            _collectionsFor = new Cache<Type, IList<CollectionConfiguration>>(
                type => _cache.GetPropertiesFor(type)
                    .Values
                    .Where(x => x.PropertyType.IsGenericEnumerable())
                    .Select(x => _byAccessor[new SingleProperty(x)])
                    .ToList());
        }


        public CollectionConfiguration Register(Accessor accessor)
        {
            var configuration = new CollectionConfiguration(accessor);
            _byAccessor[accessor] = configuration;
            return configuration;
        }

        public ObjectBlockSettings Include(Type type)
        {
            var collections = _collectionsFor[type];
            return this;
        }

        public void Ignore(Accessor accessor)
        {
            Ignore(new ExplicitIgnorePolicy(accessor));
        }

        public void Ignore(IIgnoreAccessorPolicy policy)
        {
            _ignored.Add(policy);
        }

        string IObjectBlockSettings.Collection(Type type, string key)
        {
            var map = _collectionsFor[type].SingleOrDefault(x => x.Accessor.Name.EqualsIgnoreCase(key));
            return map != null ? map.Name : key;
        }

        Accessor IObjectBlockSettings.ImplicitValue(Type type, string key)
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
            return _collectionsFor.GetAllKeys()
                .SelectMany(x => _collectionsFor[x])
                .Where(x => x.Implicit != null);
        }

        IEnumerable<string> IObjectBlockSettings.KnownCollectionNames()
        {
            return _collectionsFor.GetAllKeys()
                .SelectMany(x => _collectionsFor[x])
                .Select(x => x.Name)
                .Distinct();
        }

        bool IObjectBlockSettings.ShouldIgnore(object target, Accessor accessor)
        {
            var policies = _ignored.Where(x => x.Matches(accessor)).ToList();
            if (!policies.Any()) return false;

            return policies.Any(x => x.Ignore(target, accessor));
        }

        Accessor IObjectBlockSettings.FindImplicitValue(Type type)
        {
            return ImplicitFor(type);
        }

        Type IObjectBlockSettings.FindCollectionType(Type type, string key)
        {
            var collectionMap = _collectionsFor[type].SingleOrDefault(x => x.Name.EqualsIgnoreCase(key));
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

        public IgnorePropertyExpression<TTarget> Ignore<TTarget>(Expression<Func<T, TTarget>> expression)
        {
            var accessor = ReflectionHelper.GetAccessor(expression);
            var policy = new ConfiguredIgnorePolicy<TTarget>(accessor);

            Ignore(policy);

            return new IgnorePropertyExpression<TTarget>(policy);
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

        public class IgnorePropertyExpression<TTarget>
        {
            private readonly ConfiguredIgnorePolicy<TTarget> _policy;

            public IgnorePropertyExpression(ConfiguredIgnorePolicy<TTarget> policy)
            {
                _policy = policy;
            }

            public void When(Predicate<TTarget> predicate)
            {
                _policy.Predicate = predicate;
            }
        }
    }
}