using System;
using System.Linq.Expressions;
using FubuCore;
using FubuObjectBlocks.Writing;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests.Writing
{
    [TestFixture]
    public class SimplePropertyWriterTester
    {
        [Test]
        public void matches_string_properties()
        {
            matches(x => x.StringProp).ShouldBeTrue();
        }

        [Test]
        public void matches_int_properties()
        {
            matches(x => x.IntProp).ShouldBeTrue();
        }

        [Test]
        public void matches_decimal_properties()
        {
            matches(x => x.DecimalProp).ShouldBeTrue();
        }

        [Test]
        public void matches_double_properties()
        {
            matches(x => x.DoubleProp).ShouldBeTrue();
        }

        [Test]
        public void matches_enum_properties()
        {
            matches(x => x.EnumProp).ShouldBeTrue();
        }

        [Test]
        public void matches_float_properties()
        {
            matches(x => x.FloatProp).ShouldBeTrue();
        }

        [Test]
        public void no_match_for_complex_types()
        {
            matches(x => x.Nested).ShouldBeFalse();
        }

        [Test]
        public void no_match_for_collections()
        {
            matches(x => x.Collection).ShouldBeFalse();
        }

        [Test]
        public void creates_the_property_block()
        {
            var target = new WritingTarget {StringProp = "Test"};
            var context = contextFor(x => x.StringProp, target);

            var block = new PropertyBlockWriter().Write(context).As<PropertyBlock>();
            block.Name.ShouldEqual("stringProp");
            block.Value.ShouldEqual("Test");
        }

        private bool matches(Expression<Func<WritingTarget, object>> expression)
        {
            return new PropertyBlockWriter().Matches(contextFor(expression, new WritingTarget()));
        }

        private BlockWritingContext contextFor(Expression<Func<WritingTarget, object>> expression, WritingTarget target)
        {
            return BlockWritingContext.ContextFor(expression, target);
        }   
    }
}