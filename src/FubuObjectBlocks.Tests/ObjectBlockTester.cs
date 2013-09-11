using System.Collections.Generic;
using FubuObjectBlocks.Formatting;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuObjectBlocks.Tests
{
    [TestFixture]
    public class ObjectBlockTester
    {
        [Test]
        public void sorting_replaces_the_underlying_collection()
        {
            var b1 = MockRepository.GenerateStub<IBlock>();
            var b2 = MockRepository.GenerateStub<IBlock>();

            var sorter = new StubBlockSorter(b1, b2);

            var block = new ObjectBlock("Test");
            block.AddBlock(new PropertyBlock("Prop1"));
            block.AddBlock(new PropertyBlock("Prop2"));

            block.Sort(sorter);

            block.Blocks.ShouldHaveTheSameElementsAs(b1, b2);
        }

        public class StubBlockSorter : IBlockSorter
        {
            private readonly IEnumerable<IBlock> _blocks;

            public StubBlockSorter(params IBlock[] blocks)
            {
                _blocks = blocks;
            }

            public IEnumerable<IBlock> Sort(IEnumerable<IBlock> blocks)
            {
                return _blocks;
            }
        }
    }
}