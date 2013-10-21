using System;
using FubuCore.Reflection;

namespace FubuObjectBlocks
{
    public interface IIgnoreAccessorPolicy
    {
        bool Matches(Accessor accessor);
        bool Ignore(object target, Accessor accessor);
    }

    public class ExplicitIgnorePolicy : IIgnoreAccessorPolicy
    {
        private readonly Accessor _accessor;

        public ExplicitIgnorePolicy(Accessor accessor)
        {
            _accessor = accessor;
        }

        public bool Matches(Accessor accessor)
        {
            return _accessor.Equals(accessor);
        }

        public bool Ignore(object target, Accessor accessor)
        {
            return true;
        }
    }

    public class ConfiguredIgnorePolicy<T> : IIgnoreAccessorPolicy
    {
        private readonly Accessor _accessor;

        public ConfiguredIgnorePolicy(Accessor accessor)
        {
            _accessor = accessor;
            Predicate = x => true;
        }

        public Predicate<T> Predicate { get; set; } 

        public bool Matches(Accessor accessor)
        {
            return _accessor.Equals(accessor);
        }

        public bool Ignore(object target, Accessor accessor)
        {
            var value = (T)accessor.GetValue(target);
            return Predicate(value);
        }
    }
}