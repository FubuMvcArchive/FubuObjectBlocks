using System;

namespace FubuObjectBlocks
{
    public class BlockSeparator : IBlock
    {
        public string ToString(int indent = 0)
        {
            return Environment.NewLine;
        }

        public string Name { get { return string.Empty; } }
    }
}