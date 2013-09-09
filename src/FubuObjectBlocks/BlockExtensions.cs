using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;

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

        public static string[] Split(this string input, string delimiter)
        {
            return input.Split(new[] {delimiter}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}