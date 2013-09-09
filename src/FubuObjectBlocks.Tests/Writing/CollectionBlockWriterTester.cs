using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuObjectBlocks.Writing;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests.Writing
{
    [TestFixture]
    public class CollectionBlockWriterTester
    {
        [Test]
        public void no_match_on_others()
        {
            matches(x => x.StringProp).ShouldBeFalse();
        }

        [Test]
        public void match_on_enumerables()
        {
            matches(x => x.Collection).ShouldBeTrue();
        }

        [Test]
        public void creates_the_collection_block()
        {
            var target = new WritingTarget
            {
                Collection = new List<WritingTarget>()
                {
                    new WritingTarget { StringProp = "One"},
                    new WritingTarget { StringProp = "Two"}
                }
            };

            var context = contextFor(x => x.Collection, target);

            var block = new CollectionBlockWriter().Write(context).As<CollectionBlock>();
            block.Name.ShouldEqual("collection");

            block.Blocks[0].As<ObjectBlock>().Blocks[0].As<PropertyBlock>().Value.ShouldEqual("One");
            block.Blocks[1].As<ObjectBlock>().Blocks[0].As<PropertyBlock>().Value.ShouldEqual("Two");
        }

        private bool matches(Expression<Func<WritingTarget, object>> expression)
        {
            return new CollectionBlockWriter().Matches(contextFor(expression, new WritingTarget()));
        }

        private BlockWritingContext contextFor(Expression<Func<WritingTarget, object>> expression, WritingTarget target)
        {
            return BlockWritingContext.ContextFor(expression, target);
        }   
    }
}