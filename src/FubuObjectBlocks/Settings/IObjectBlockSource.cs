using System.Collections.Generic;

namespace FubuObjectBlocks.Settings
{
    public interface IObjectBlockSource
    {
        IEnumerable<ObjectBlock> Blocks();
    }
}