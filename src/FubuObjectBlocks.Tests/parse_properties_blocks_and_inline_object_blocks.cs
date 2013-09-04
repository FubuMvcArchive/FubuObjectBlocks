using System.Collections.Generic;
using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests
{
    [TestFixture]
    public class parse_properties_blocks_and_inline_object_blocks
    {
        private ParsingScenario theScenario;

        [SetUp]
        public void SetUp()
        {
            theScenario = ParsingScenario.Create(scenario =>
            {
                scenario.WriteLine("test1 'immediate value'");
                scenario.WriteLine("");
                scenario.WriteLine("nestedType:");
                scenario.WriteLine("  nestedProperty 'string value'");
                scenario.WriteLine("");
                scenario.WriteLine("feed 'some url', mode: 'float', stability: 'released'");
                scenario.WriteLine("");
                scenario.WriteLine("test2 'another value'");
                
                
            });
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Dispose();
        }

        private IEnumerable<IBlock> theBlocks { get { return theScenario.Read().Blocks; } }

        private ObjectBlock theInlineNestedObject
        {
            get { return theBlocks.Skip(2).First() as ObjectBlock; }
        }

        [Test]
        public void reads_the_first_immediate_property()
        {
            var property = theBlocks.First() as PropertyBlock;
            property.Name.ShouldEqual("test1");
            property.Value.ShouldEqual("immediate value");
        }

        [Test]
        public void reads_the_nested_object_property()
        {
            var nestedObject = theBlocks.Skip(1).First() as ObjectBlock;
            nestedObject.Name.ShouldEqual("nestedType");

            var properties = nestedObject.GetBlocks<PropertyBlock>().ToArray();

            properties[0].Name.ShouldEqual("nestedProperty");
            properties[0].Value.ShouldEqual("string value");
        }

        [Test]
        public void reads_the_last_immediate_property()
        {
            var property = theBlocks.Skip(3).First() as PropertyBlock;
            property.Name.ShouldEqual("test2");
            property.Value.ShouldEqual("another value");
        }

        [Test]
        public void reads_the_property_name_and_value()
        {
            var inlineNested = theInlineNestedObject;
            inlineNested.Name.ShouldEqual("feed");
            inlineNested.ImplicitValue.ShouldEqual("some url");
        }

        [Test]
        public void reads_the_nested_properties()
        {
            var properties = theInlineNestedObject.GetBlocks<PropertyBlock>().ToArray();

            properties[0].Name.ShouldEqual("mode");
            properties[0].Value.ShouldEqual("float");

            properties[1].Name.ShouldEqual("stability");
            properties[1].Value.ShouldEqual("released");
        }
    }
}