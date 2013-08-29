using System;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests
{
    [TestFixture]
    public class write_a_single_object_block_with_nested_values
    {
        [Test]
        public void writes_the_values()
        {
            var block = new ObjectBlock();
            block.AddBlock(new PropertyBlock("prop1") { Value = "val1" } );
            
            var nested = new ObjectBlock();
            nested.AddBlock(new PropertyBlock("nestedProp1") { Value = "val2" } );

            block.AddBlock(nested);

            block.ToString().ShouldEqual("prop1 'val1'{0}nestedProperty:{0}  nestedProp1 'val2'{0}".ToFormat(Environment.NewLine));
        }
    }
}