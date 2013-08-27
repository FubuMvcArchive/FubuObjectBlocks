using System.Collections.Generic;
using System.Linq;

namespace FubuObjectBlocks
{
    public class PropertyBlock
    {
        private readonly IList<ObjectBlock> _blocks = new List<ObjectBlock>();

        public PropertyBlock()
        {
        }

        public PropertyBlock(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public ObjectBlock Block
        {
            get { return _blocks.First(); }
            set
            {
                _blocks.Clear();
                _blocks.Add(value);
            }
        }
        
        public void Clear()
        {
            _blocks.Clear();
        }

        public IEnumerable<ObjectBlock> Blocks { get { return _blocks; } }

        public void AddBlock(ObjectBlock block)
        {
            _blocks.Add(block);
        }

        protected bool Equals(PropertyBlock other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PropertyBlock) obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        public static PropertyBlock ForValue(string name, string value)
        {
            return new PropertyBlock(name) { Block = new ObjectBlock { Value = value }};
        }
    }
}