using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests
{
    [TestFixture]
    public class parse_a_single_object_block
    {
        private ParsingScenario theScenario;

        [SetUp]
        public void SetUp()
        {
            theScenario = ParsingScenario.Create(scenario =>
            {
                scenario.WriteLine("# Comment");
                scenario.WriteLine("blockProperty:");
                scenario.WriteLine("  property1 'string value'");
                scenario.WriteLine("  property2 'another string value'");
            });
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Dispose();
        }

        private ObjectBlock theBlock { get { return theScenario.Read(); } }

        [Test]
        public void reads_the_block_property()
        {
            var property = theBlock.Properties.Single();
            property.Name.ShouldEqual("blockProperty");
            property.Block.ShouldBeOfType<ObjectBlock>();
        }

        [Test]
        public void reads_the_properties_of_the_block()
        {
            var block = theBlock.Properties.Single().Block;
            var properties = block.Properties.ToArray();

            properties[0].Name.ShouldEqual("property1");
            properties[0].Block.Value.ShouldEqual("string value");

            properties[1].Name.ShouldEqual("property2");
            properties[1].Block.Value.ShouldEqual("another string value");
        }
    }
}