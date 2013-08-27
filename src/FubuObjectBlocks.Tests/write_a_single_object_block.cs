using System;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests
{
    [TestFixture]
    public class write_a_single_object_block
    {
        [Test]
        public void writes_the_values()
        {
            var block = new ObjectBlock();
            block.AddProperty(new PropertyBlock("prop1") { Block = new ObjectBlock { Value = "val1"} });
            block.AddProperty(new PropertyBlock("prop2") { Block = new ObjectBlock { Value = "val2" } });

            block.Write().ShouldEqual("prop1 'val1'{0}prop2 'val2'{0}".ToFormat(Environment.NewLine));
        }
    }
}