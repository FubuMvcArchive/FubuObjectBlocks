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
                scenario.WriteLine("test1: 'immediate value'");
                scenario.WriteLine("");
                scenario.WriteLine("nestedType:");
                scenario.WriteLine("  nestedProperty: 'string value'");
                scenario.WriteLine("");
                scenario.WriteLine("test2: 'another value'");
            });
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Dispose();
        }

        private IBlock[] theBlocks { get { return theScenario.Read().Blocks.ToArray(); } }

        [Test]
        public void reads_the_first_immediate_property()
        {
            var property = theBlocks[0] as PropertyBlock;
            property.ShouldNotBeNull();
            property.Name.ShouldEqual("test1");
            property.Value.ShouldEqual("immediate value");
        }

        [Test]
        public void reads_the_nested_property()
        {
            var nestedObject = theBlocks[1] as ObjectBlock;
            nestedObject.Name.ShouldEqual("nestedType");
            
            var properties = nestedObject.GetBlocks<PropertyBlock>().ToArray();

            properties[0].Name.ShouldEqual("nestedProperty");
            properties[0].Value.ShouldEqual("string value");
        }

        [Test]
        public void reads_the_last_immediate_property()
        {
            var property = theBlocks[2] as PropertyBlock;
            property.Name.ShouldEqual("test2");
            property.Value.ShouldEqual("another value");
        }
    }
}