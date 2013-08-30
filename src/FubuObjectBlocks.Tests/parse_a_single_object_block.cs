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

        private ObjectBlock theBlock { get { return theScenario.Read().Blocks.Single() as ObjectBlock; } }

        [Test]
        public void reads_the_object_block_property()
        {
            var block = theBlock;
            block.ShouldNotBeNull();
            block.Name.ShouldEqual("blockProperty");
        }

        [Test]
        public void reads_the_properties_of_the_block()
        {
            var properties = theBlock.GetBlocks<PropertyBlock>().ToArray();

            properties[0].Name.ShouldEqual("property1");
            properties[0].Value.ShouldEqual("string value");

            properties[1].Name.ShouldEqual("property2");
            properties[1].Value.ShouldEqual("another string value");
        }
    }
}