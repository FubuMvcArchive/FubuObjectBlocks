using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests
{
    [TestFixture]
    public class parse_an_object_block_with_nested_types_and_immediate_properties
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
            var property = theProperties[2];
            property.Name.ShouldEqual("test2");
            property.Block.Value.ShouldEqual("another value");
        }
    }
}