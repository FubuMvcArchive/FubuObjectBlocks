using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;

namespace FubuObjectBlocks
{
    //TODO: this seems to only apply to Collections, name it CollectionBlockSettings?
    public class BlockSettingsAttribute : Attribute
    {
        private static readonly TypeDescriptorCache Cache;

        static BlockSettingsAttribute()
        {
            Cache = new TypeDescriptorCache();
        }

        public string ExpressAs { get; set; }
        public string ImplicitProperty { get; set; }

        public CollectionConfiguration ToConfiguration(Type type, PropertyInfo property)
        {
            if (!property.PropertyType.Closes(typeof (IEnumerable<>)))
            {
                throw new InvalidOperationException("BlockSettingsAttribute is only valid for generic collections");
            }

            var configuration = new CollectionConfiguration(new SingleProperty(property))
            {
                Name = ExpressAs
            };

            if (ImplicitProperty.IsNotEmpty())
            {
                var targetType = property.PropertyType.FindInterfaceThatCloses(typeof(IEnumerable<>)).GetGenericArguments()[0];
                var target = Cache.GetPropertiesFor(targetType).SingleOrDefault(x => x.Key.EqualsIgnoreCase(ImplicitProperty)).Value;
                if (target == null)
                {
                    throw new InvalidOperationException("Could not find property: " + ImplicitProperty);
                }

                configuration.Implicit = new SingleProperty(target);
            }

            return configuration;
        }
    }
}