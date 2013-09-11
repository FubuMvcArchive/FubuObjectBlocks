using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuObjectBlocks.Settings
{
    public class ObjectBlockCollection
    {
        private readonly IEnumerable<ObjectBlock> _blocks;

        public ObjectBlockCollection(IEnumerable<ObjectBlock> blocks)
        {
            _blocks = blocks;
        }

        public bool Has(string name)
        {
            return Find(name) != null;
        }

        public ObjectBlock Find(string name)
        {
            return _blocks.SingleOrDefault(x => x.Name.EqualsIgnoreCase(name));
        }
    }
}