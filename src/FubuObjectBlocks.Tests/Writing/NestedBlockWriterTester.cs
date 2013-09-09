using System;
using System.Linq.Expressions;
using FubuCore;
using FubuObjectBlocks.Writing;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests.Writing
{
    [TestFixture]
    public class NestedBlockWriterTester
    {
        [Test]
        public void always_match()
        {
            new NestedBlockWriter().Matches(null).ShouldBeTrue();
        }

        [Test]
        public void creates_the_nested_block()
        {
            var target = new WritingTarget
            {
                Nested = new WritingTarget
                {
                    StringProp = "Test"
                }
            };

            var context = contextFor(x => x.Nested, target);

            var block = new NestedBlockWriter().Write(context).As<ObjectBlock>();
            block.Name.ShouldEqual("nested");
        }

        private BlockWritingContext contextFor(Expression<Func<WritingTarget, object>> expression, WritingTarget target)
        {
            return BlockWritingContext.ContextFor(expression, target);
        }   
    }
}