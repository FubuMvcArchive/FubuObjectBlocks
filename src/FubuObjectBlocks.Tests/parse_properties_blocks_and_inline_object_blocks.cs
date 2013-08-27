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

        private ObjectBlock theBlock { get { return theScenario.Read(); } }
        private PropertyBlock[] theProperties { get { return theBlock.Properties.ToArray(); } }
        private PropertyBlock theInlineProperty
        {
            get { return theProperties[2]; }
        }

        [Test]
        public void reads_the_first_immediate_property()
        {
            var property = theProperties[0];
            property.Name.ShouldEqual("test1");
            property.Block.Value.ShouldEqual("immediate value");
        }

        [Test]
        public void reads_the_nested_property()
        {
            var property = theProperties[1];
            property.Name.ShouldEqual("nestedType");

            var properties = property.Block.Properties.ToArray();

            properties[0].Name.ShouldEqual("nestedProperty");
            properties[0].Block.Value.ShouldEqual("string value");
        }

        [Test]
        public void reads_the_last_immediate_property()
        {
            var property = theProperties[3];
            property.Name.ShouldEqual("test2");
            property.Block.Value.ShouldEqual("another value");
        }

        [Test]
        public void reads_the_property_name_and_value()
        {
            var property = theInlineProperty;
            property.Name.ShouldEqual("feed");
            property.Block.Value.ShouldEqual("some url");
        }

        [Test]
        public void reads_the_nested_properties()
        {
            var properties = theInlineProperty.Block.Properties.ToArray();

            properties[0].Name.ShouldEqual("mode");
            properties[0].Block.Value.ShouldEqual("float");

            properties[1].Name.ShouldEqual("stability");
            properties[1].Block.Value.ShouldEqual("released");
        }
    }
}