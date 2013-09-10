using FubuCore.Reflection;

namespace FubuObjectBlocks
{
    public class BlockToken
    {
        public static readonly BlockToken Empty = new BlockToken((string)null);

        private readonly string _value;
        private readonly Accessor _accessor;

        public BlockToken(string value)
            : this(value, null)
        {
        }

        public BlockToken(Accessor accessor)
            : this(accessor.Name, accessor)
        {
        }

        public BlockToken(string value, Accessor accessor)
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

        protected bool Equals(BlockToken other)
        {
            return string.Equals(_value, other._value) && Equals(_accessor, other._accessor);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BlockToken) obj);
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