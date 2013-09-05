using FubuCore;

namespace FubuObjectBlocks
{
    public class PropertyBlock : IBlock
    {
        public PropertyBlock(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return ToString(0);
        }

        public string ToString(int indent)
        {
            return BlockIndenter.Indent("{0}: '{1}'".ToFormat(Name, Value), indent);
        }
    }
}