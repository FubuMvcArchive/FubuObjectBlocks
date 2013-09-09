using FubuCore.Reflection;

namespace FubuObjectBlocks
{
    public class BlockName
    {
        public static readonly BlockName Empty = new BlockName((string)null);

        private readonly string _value;
        private readonly Accessor _accessor;

        public BlockName(string value)
            : this(value, null)
        {
        }

        public BlockName(Accessor accessor)
            : this(accessor.Name, accessor)
        {
        }

        public BlockName(string value, Accessor accessor)
        {
            _value = value;
            _accessor = accessor;
        }

        public string Value
        {
            get { return _value; }
        }

        public Accessor Accessor
        {
            get { return _accessor; }
        }

        public bool IsEmpty()
        {
            return Empty.Equals(this);
        }

        protected bool Equals(BlockName other)
        {
            return string.Equals(_value, other._value) && Equals(_accessor, other._accessor);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BlockName) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_value != null ? _value.GetHashCode() : 0)*397) ^ (_accessor != null ? _accessor.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return _value;
        }
    }
}