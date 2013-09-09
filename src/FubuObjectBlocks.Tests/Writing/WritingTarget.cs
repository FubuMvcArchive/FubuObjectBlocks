using System.Collections.Generic;

namespace FubuObjectBlocks.Tests.Writing
{
    public class WritingTarget
    {
        public string StringProp { get; set; }
        public int IntProp { get; set; }
        public decimal DecimalProp { get; set; }
        public double DoubleProp { get; set; }
        public SimpleEnum EnumProp { get; set; }
        public float FloatProp { get; set; }

        public IEnumerable<WritingTarget> Collection { get; set; }

        public WritingTarget Nested { get; set; }
    }
}