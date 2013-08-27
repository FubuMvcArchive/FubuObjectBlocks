using FubuCore.Reflection;

namespace FubuObjectBlocks
{
    public class CollectionConfiguration
    {
        private readonly Accessor _accessor;

        public CollectionConfiguration(Accessor accessor)
        {
            _accessor = accessor;

            Name = _accessor.Name;
        }

        public Accessor Accessor { get { return _accessor; } }
        public string Name { get; set; }
        public Accessor Implicit { get; set; }
    }
}